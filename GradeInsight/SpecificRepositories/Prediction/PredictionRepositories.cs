using GradeInsight.Data;
using GradeInsight.ViewModel;
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
                                      join s in _context.Semester.AsNoTracking() on c.SemesterId equals s.SemesterId
                                      join e in _context.ExamType.AsNoTracking() on m.ExamTypeId equals e.ExamTypeId
                                      join f in _context.Faculty.AsNoTracking() on s.FacultyId equals f.FacultyId
                                      where m.Mark > 0
                                      group m by new { m.StudentId, m.CourseId } into g
                                      select new
                                      {
                                          StudentId = g.Key.StudentId,
                                          CourseId = g.Key.CourseId,
                                          InternalMarks = g.Where(x => x.ExamTypeId == 1 && x.CourseId == g.Key.CourseId)
                                                           .Select(x => x.Mark).FirstOrDefault(),
                                          PreboardMarks = g.Where(x => x.ExamTypeId == 2 && x.CourseId == g.Key.CourseId)
                                                           .Select(x => x.Mark).FirstOrDefault(),
                                          WeightedMarks = (0.4 * g.Where(x => x.ExamTypeId == 1 && x.CourseId == g.Key.CourseId)
                                                                   .Select(x => x.Mark).FirstOrDefault()) +
                                                          (0.6 * g.Where(x => x.ExamTypeId == 2 && x.CourseId == g.Key.CourseId)
                                                                   .Select(x => x.Mark).FirstOrDefault())
                                      }).ToListAsync();

            if (!trainingData.Any())
                throw new InvalidOperationException("No data available for training.");

            lock (_lock)
                     {
                var courseMapping = trainingData.Select(d => d.CourseId).Distinct()
                                       .Select((id, index) => new { id, index })
                                       .ToDictionary(x => x.id, x => x.index);
                _courseMapping = courseMapping;


                double[][] inputs = trainingData.Select(d => new double[]
                                      {
                                       d.InternalMarks * 0.4,  
                                       d.PreboardMarks * 0.6,
                                        courseMapping[d.CourseId] 
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

        public async Task<object> TrainModels()
        {
            // Fetch training data
            var trainingData = await (from m in _context.Marks.AsNoTracking()
                                      join c in _context.Course.AsNoTracking() on m.CourseId equals c.CourseId
                                      join s in _context.Semester.AsNoTracking() on c.SemesterId equals s.SemesterId
                                      join e in _context.ExamType.AsNoTracking() on m.ExamTypeId equals e.ExamTypeId
                                      join f in _context.Faculty.AsNoTracking() on s.FacultyId equals f.FacultyId
                                      where m.Mark > 0
                                      group m by new { m.StudentId, m.CourseId } into g
                                      select new
                                      {
                                          StudentId = g.Key.StudentId,
                                          CourseId = g.Key.CourseId,
                                          InternalMarks = g.Where(x => x.ExamTypeId == 1 && x.CourseId == g.Key.CourseId)
                                                           .Select(x => x.Mark).FirstOrDefault(),
                                          PreboardMarks = g.Where(x => x.ExamTypeId == 2 && x.CourseId == g.Key.CourseId)
                                                           .Select(x => x.Mark).FirstOrDefault(),
                                          WeightedMarks = (0.4 * g.Where(x => x.ExamTypeId == 1 && x.CourseId == g.Key.CourseId)
                                                                   .Select(x => x.Mark).FirstOrDefault()) +
                                                          (0.6 * g.Where(x => x.ExamTypeId == 2 && x.CourseId == g.Key.CourseId)
                                                                   .Select(x => x.Mark).FirstOrDefault())
                                      }).ToListAsync();

            // Check if no data is found
            if (!trainingData.Any())
                throw new InvalidOperationException("No data available for training.");

            // Lock and create the course mapping
            lock (_lock)
            {
                var courseMapping = trainingData.Select(d => d.CourseId).Distinct()
                                               .Select((id, index) => new { id, index })
                                               .ToDictionary(x => x.id, x => x.index);
                _courseMapping = courseMapping;

                // Prepare inputs (InternalMarks and PreboardMarks with weights) and outputs (WeightedMarks)
                double[][] inputs = trainingData.Select(d => new double[]
                {
            d.InternalMarks * 0.4,  
            d.PreboardMarks * 0.6,  
            courseMapping[d.CourseId] 
                }).ToArray();

                double[] outputs = trainingData.Select(d => d.WeightedMarks).ToArray();

              

                // Return the training data, including the mappings and the processed dataset
                return new
                {
                    TrainingData = trainingData,
                    CourseMapping = _courseMapping,
                    Inputs = inputs,
                    Outputs = outputs
                };
            }
        }

        public  Task<double> PredictMarks(PredictionInitialDataViewModel input)
        {
            lock (_lock)
            {
                if (_coefficients == null)
                    throw new InvalidOperationException("Model has not been trained yet.");

                double[] inputData = new double[]
                {
            input.InternalMarks * 0.4,
            input.PreboardMarks * 0.6,
             _courseMapping[input.CourseId]
                };

              

                // Fetch the course name from the database
                var courseName = _context.Course
                                         .Where(c => c.CourseId == input.CourseId)
                                         .Select(c => c.CourseName)
                                         .FirstOrDefault();

                double predictedMarks = Predict(inputData);



                return Task.FromResult(predictedMarks);
            }
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
