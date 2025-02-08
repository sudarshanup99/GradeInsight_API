using GradeInsight.Data;
using GradeInsight.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GradeInsight.SpecificRepositories.Prediction
{
    public class PredictionRepositories : IPredictionRepositories
    {
        private readonly GradeInsightContext _context;

      
        private static readonly object _lock = new object();
        private static Dictionary<int, int> _courseMapping;
        private static Dictionary<string, int> _examTypeMapping;
        private static double[] _coefficients;

        public PredictionRepositories(GradeInsightContext context)
        {
            _context = context;
        }

        public async Task TrainModel()
        {
            var trainingData = await (from m in _context.Marks.AsNoTracking()
                                      join c in _context.Course.AsNoTracking() on m.CourseId equals c.CourseId
                                      where m.Mark > 0
                                      group m by new { m.StudentId } into g
                                      select new
                                      {
                                          InternalMarks = g.Where(x => x.ExamTypeId == 1).Sum(x => x.Mark),  // Sum of internal marks (ExamTypeId == 1)
                                          PreboardMarks = g.Where(x => x.ExamTypeId == 2).Sum(x => x.Mark),  // Sum of preboard marks (ExamTypeId == 2)
                                          WeightedMarks = (0.4 * g.Where(x => x.ExamTypeId == 1).Sum(x => x.Mark)) + // Sum of internal marks weighted by 0.4
                                                          (0.6 * g.Where(x => x.ExamTypeId == 2).Sum(x => x.Mark))  // Sum of preboard marks weighted by 0.6
                                      }).ToListAsync();
            if (!trainingData.Any())
                throw new InvalidOperationException("No data available for training.");

            lock (_lock)
                     {
             

                double[][] inputs = trainingData.Select(d => new double[]
                                      {
                                       d.InternalMarks * 0.4,  
                                       d.PreboardMarks * 0.6,
                                      
                                         }).ToArray();

                double[] outputs = trainingData.Select(d => d.WeightedMarks).ToArray();
               

                
                Train(inputs, outputs);
                
            }
        }

        private void Train(double[][] inputs, double[] outputs)
        {
            int n = inputs.Length;
            int m = inputs[0].Length;
            double[] beta = new double[m + 1];

            double[][] X = new double[n][];
            for (int i = 0; i < n; i++)
            {
                X[i] = new double[m + 1];
                X[i] = new double[m + 1];
                X[i][0] = 1; // Bias term
                for (int j = 0; j < m; j++)
                {
                    X[i][j + 1] = inputs[i][j];
                }
            }

            double[][] XT = TransposeMatrix(X);
            double[][] XTX = MultiplyMatrices(XT, X);
            double[][] XTXInv = InvertMatrixWithRegularization(XTX);
            double[][] outputsMatrix = outputs.Select(o => new double[] { o }).ToArray();
            double[][] XTY = MultiplyMatrices(XT, outputsMatrix);

            // Update global coefficients safely
           
         _coefficients = MultiplyMatrices(XTXInv, XTY).Select(a => a[0]).ToArray();
            
        }

        private double Predict(double[] input)
        {
            if (_coefficients == null)
                throw new InvalidOperationException("Model has not been trained yet.");

            double result = _coefficients[0]; // Bias
            for (int i = 0; i < input.Length; i++)
            {
                result += _coefficients[i + 1] * input[i];
            }
            return result;
        }

       

        public  Task<object> PredictMarks(PredictionInitialDataViewModel input)
        {
            lock (_lock)
            {
                if (_coefficients == null)
                    throw new InvalidOperationException("Model has not been trained yet.");

            }

                double[] inputData = new double[]
                {
                 input.InternalMarks * 0.4,
                  input.PreboardMarks * 0.6,
             
                };

              

                // Fetch the course name from the database
                

                double predictedMarks = Predict(inputData);
                var internalMarks =  _context.Marks
                           .Where(m => m.StudentId == input.StudentId && m.ExamTypeId == 1)
                           .Sum(m => m.Mark);

                var preboardMarks = _context.Marks
                                    .Where(m => m.StudentId == input.StudentId && m.ExamTypeId == 2)
                                    .Sum(m => m.Mark);

            // If no marks found, return 422 (Unprocessable Entity)

            if (internalMarks == 0 && preboardMarks == 0)
            {
                return Task.FromResult<object>(new { Message = "Actual marks not found." });
            }

            // Calculate MAE (Mean Absolute Error)
            double actualMarks = internalMarks + preboardMarks;
            double mae = Math.Abs(predictedMarks - actualMarks);

            // Compute Accuracy as: Accuracy = (1 - (MAE / Actual Marks)) * 100
            double accuracy = actualMarks > 0 ? (1 - (mae / actualMarks)) * 100 : 0;

            return Task.FromResult<object>(new
            {
                PredictedMarks = predictedMarks,
                Accuracy = accuracy
            });

        }




        private double[][] TransposeMatrix(double[][] matrix)
        {
            int rows = matrix.Length;       // Number of rows in original matrix
            int cols = matrix[0].Length;    // Number of columns in original matrix

            double[][] transposed = new double[cols][];  // Transposed matrix

            for (int i = 0; i < cols; i++)
            {
                transposed[i] = new double[rows];  // Initialize each row in transposed matrix
                for (int j = 0; j < rows; j++)
                {
                    transposed[i][j] = matrix[j][i];  // Swap row and column
                }
            }

            return transposed;
        }

        private double[][] MultiplyMatrices(double[][] a, double[][] b)
        {
            int rowsA = a.Length;         
            int colsA = a[0].Length;     
            int colsB = b[0].Length;      

            if (colsA != b.Length)
            {
                throw new ArgumentException("Matrix multiplication is not possible. Number of columns in A must match number of rows in B.");
            }

            // Initialize the result matrix with dimensions (rowsA x colsB)
            double[][] result = new double[rowsA][];
            for (int i = 0; i < rowsA; i++)
            {
                result[i] = new double[colsB]; // Each row has 'colsB' columns
                for (int j = 0; j < colsB; j++)
                {
                    result[i][j] = 0;  
                    for (int k = 0; k < colsA; k++) // colsA == rowsB
                    {
                        result[i][j] += a[i][k] * b[k][j];  // Compute dot product
                    }
                }
            }

            return result;
        }
        public static double[][] InvertMatrixWithRegularization(double[][] matrix)
        {
            int n = matrix.Length;
            double regularizationFactor = 1e-5;

           
            for (int i = 0; i < n; i++)
            {
                matrix[i][i] += regularizationFactor;
            }

            
            return InvertMatrix(matrix);
        }
        private static double[][] InvertMatrix(double[][] matrix)
        {
            int n = matrix.Length; 

          
            double[][] augmented = new double[n][];
            for (int i = 0; i < n; i++)
            {
                augmented[i] = new double[2 * n];
                for (int j = 0; j < n; j++)
                {
                    augmented[i][j] = matrix[i][j];  
                }
                augmented[i][n + i] = 1;  
            }

            
            for (int i = 0; i < n; i++)
            {
                // Find pivot (make sure it's non-zero)
                if (augmented[i][i] == 0)
                {
                    // Swap with a non-zero row
                    int swapRow = i + 1;
                    while (swapRow < n && augmented[swapRow][i] == 0) swapRow++;
                    if (swapRow == n) throw new InvalidOperationException("Matrix is singular and cannot be inverted.");

                    // Swap rows
                    double[] temp = augmented[i];
                    augmented[i] = augmented[swapRow];
                    augmented[swapRow] = temp;
                }

      
                double pivot = augmented[i][i];
                for (int j = 0; j < 2 * n; j++)
                {
                    augmented[i][j] /= pivot;
                }

                // Eliminate other rows
                for (int k = 0; k < n; k++)
                {
                    if (k == i) continue; // Skip the pivot row
                    double factor = augmented[k][i];
                    for (int j = 0; j < 2 * n; j++)
                    {
                        augmented[k][j] -= factor * augmented[i][j];
                    }
                }
            }

            // Extract the inverse matrix from the augmented matrix
            double[][] inverse = new double[n][];
            for (int i = 0; i < n; i++)
            {
                inverse[i] = new double[n];
                for (int j = 0; j < n; j++)
                {
                    inverse[i][j] = augmented[i][j + n];  // Extract right half
                }
            }

            return inverse;
        }

    }
}
