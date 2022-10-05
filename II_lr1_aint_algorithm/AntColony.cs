using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace II_lr1_aint_algorithm
{
    class AntColony
    {
        private static readonly Random random = new Random(0);

        //спецификаторы алгоритма
        private static int alpha { get; set; } // влияние феромона на направление
        private static int beta { get; set; }  // влияние расстояния между соседними узлами

        private static double rho { get; set; } // коэффициент снижения феромонов
        private static double Q { get; set; }   // коэффициент увеличения феромонов

        private static int numCities { get; set; } //число узлов графа
        private static int numAnts { get; set; } //количество муравьев
        private static int maxTime { get; set; } //максимальный путь одного муравья

        private static int[][] dists { get; set; }

        public static void Main()
        {
            try
            {
                Console.WriteLine("\nBegin Ant Colony Optimization\n");

                //объявляем изначальные переменны

                //получаем информацию из файла
                string temp;
                Console.WriteLine("Enter a filename:");
                temp = Console.ReadLine();
               

                //Выводи все показатели нашего алгоритма
                Console.WriteLine("Number cities in problem = " + numCities);
                Console.WriteLine("\nNumber ants = " + numAnts);
                Console.WriteLine("Maximum time = " + maxTime);
                Console.WriteLine("\nAlpha (pheromone influence) = " + alpha);
                Console.WriteLine("Beta (local node influence) = " + beta);
                Console.WriteLine("Rho (pheromone evaporation coefficient) = " + rho.ToString("F2"));
                Console.WriteLine("Q (pheromone deposit factor) = " + Q.ToString("F2"));

                Console.WriteLine("\nInitialing ants to random trails\n");
                int[][] ants = InitAnts(numAnts, numCities); // запускаем иуравья по случайному пути

                int[] bestTrail = BestTrail(ants, dists); // обозначаем лучший инициализированный путь
                double bestLength = Length(bestTrail, dists); // длина лучшего инициализированного пути

                //заносим информацию о первом ферромонном пути
                Console.Write("\nBest initial trail length: " + bestLength.ToString("F1") + "\n");
                Console.WriteLine("\nInitializing pheromones on trails");
                double[][] pheromones = InitPheromones(numCities);

                //проводим цикл исследований
                int time = 0;
                Console.WriteLine("\nEntering UpdateAnts - UpdatePheromones loop\n");
                while (time < maxTime)
                {
                    UpdateAnts(ants, pheromones, dists);
                    UpdatePheromones(pheromones, ants, dists);

                    int[] currBestTrail = BestTrail(ants, dists);
                    double currBestLength = Length(currBestTrail, dists);
                    if (currBestLength < bestLength)
                    {
                        bestLength = currBestLength;
                        bestTrail = currBestTrail;
                        Console.WriteLine("New best length of " + bestLength.ToString("F1") + " found at time " + time);
                    }
                    ++time;
                }

                //вывод наилучшего пути и информации о нем
                Console.WriteLine("\nBest trail found:");
                Display(bestTrail);
                Console.WriteLine("\nLength of best trail found: " + bestLength.ToString("F1"));
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                Console.ReadLine();
            }

        }
        //метод создания муравьёв и первичной инициализации их пути
        private static int[][] InitAnts(int numAnts, int numCities)
        {
            int[][] ants = new int[numAnts][];
            for (int k = 0; k < numAnts; ++k)
            {
                int start = random.Next(0, numCities);
                ants[k] = RandomTrail(start, numCities);
            }
            return ants;
        }

        private static int[] RandomTrail(int start, int numCities) //вспомогательный метод для InitAnts
        {
            int[] trail = new int[numCities];
            for (int i = 0; i < numCities; ++i) //последовательность
            {
                trail[i] = i;
            }
            for (int i = 0; i < numCities; ++i) //Перемешиваем элементы
            {
                int r = random.Next(i, numCities);
                int tmp = trail[r]; trail[r] = trail[i]; trail[i] = tmp;
            }
            int idx = IndexOfTarget(trail, start); // Устанавливаем начало в [0]
            int temp = trail[0];
            trail[0] = trail[idx];
            trail[idx] = temp;
            return trail;
        }

        private static int IndexOfTarget(int[] trail, int target) // вспомогательный метод для RandomTrail
        {
            for (int i = 0; i < trail.Length; ++i)
            {
                if (trail[i] == target)
                    return i;
            }
            throw new Exception("Target not found in IndexOfTarget");
        }

        private static double Length(int[] trail, int[][] dists) // общая длина пути
        {
            double result = 0.0;
            for (int i = 0; i < trail.Length - 1; ++i)
                result += Distance(trail[i], trail[i + 1], dists);
            return result;
        }

        private static int[] BestTrail(int[][] ants, int[][] dists) //лучший путь имеет самую короткую общую длину
        {
            double bestLength = Length(ants[0], dists);
            int idxBestLength = 0;
            for (int k = 1; k < ants.Length; ++k)
            {
                double len = Length(ants[k], dists);
                if (len < bestLength)
                {
                    bestLength = len;
                    idxBestLength = k;
                }
            }
            int numCities = ants[0].Length;
            int[] bestTrail = new int[numCities];
            ants[idxBestLength].CopyTo(bestTrail, 0);
            return bestTrail;
        }
        private static double[][] InitPheromones(int numCities)
        {
            double[][] pheromones = new double[numCities][];
            for (int i = 0; i < numCities; ++i)
                pheromones[i] = new double[numCities];
            for (int i = 0; i < pheromones.Length; ++i)
                for (int j = 0; j < pheromones[i].Length; ++j)
                    pheromones[i][j] = 0.01; // иначе же UpdateAnts -> BuiuldTrail -> NextNode -> MoveProbs => all 0.0 => throws
            return pheromones;
        }

        private static void UpdateAnts(int[][] ants, double[][] pheromones, int[][] dists) //обнолвяем состояние муравьёв
        {
            int numCities = pheromones.Length;
            for (int k = 0; k < ants.Length; ++k)
            {
                int start = random.Next(0, numCities);
                int[] newTrail = BuildTrail(start, pheromones, dists);
                ants[k] = newTrail;
            }
        }

        private static int[] BuildTrail(int start, double[][] pheromones, int[][] dists) //строит новый путь с учётом феромонов
        {
            int numCities = pheromones.Length;
            int[] trail = new int[numCities];
            bool[] visited = new bool[numCities];
            trail[0] = start;
            visited[start] = true;
            for (int i = 0; i < numCities - 1; ++i)
            {
                int cityX = trail[i];
                int next = NextCity(cityX, visited, pheromones, dists);
                trail[i + 1] = next;
                visited[next] = true;
            }
            return trail;
        }

        private static int NextCity(int cityX, bool[] visited, double[][] pheromones, int[][] dists) //определяет следующий город для муравья, который прошел некоторый путь и сейчас в городе X
        {
            double[] probs = MoveProbs(cityX, visited, pheromones, dists);
            double[] cumul = new double[probs.Length + 1];
            for (int i = 0; i < probs.Length; ++i)
                cumul[i + 1] = cumul[i] + probs[i];
            double p = random.NextDouble();
            for (int i = 0; i < cumul.Length - 1; ++i)
                if (p >= cumul[i] && p < cumul[i + 1])
                    return i;
            throw new Exception("Failure to return valid city in NextCity");
        }

        private static double[] MoveProbs(int cityX, bool[] visited, double[][] pheromones, int[][] dists) // для муравья k, расположенного в узле X, с некоторым путём, возвращает вероятность похода в каждый город
        {
            int numCities = pheromones.Length;
            double[] taueta = new double[numCities]; // включая нынешний город и пройденные города
            double sum = 0.0;
            for (int i = 0; i < taueta.Length; ++i) // i is the adjacent city
            {
                if (i == cityX)
                    taueta[i] = 0.0; // вероятность вернуться в нынешний город - 0
                else if (visited[i] == true)
                    taueta[i] = 0.0; // вероятность вернуться в уже посещённый город - 0
                else
                {
                    taueta[i] = Math.Pow(pheromones[cityX][i], alpha) * Math.Pow((1.0 / Distance(cityX, i, dists)), beta); // может быть огромным, если и значение феромонов большое
                    if (taueta[i] < 0.0001)
                        taueta[i] = 0.0001;
                    else if (taueta[i] > (double.MaxValue / (numCities * 100)))
                        taueta[i] = double.MaxValue / (numCities * 100);
                }
                sum += taueta[i];
            }

            double[] probs = new double[numCities];
            for (int i = 0; i < probs.Length; ++i)
                probs[i] = taueta[i] / sum; //будет очень больно, если сумма равна 0
            return probs;
        }

        private static void UpdatePheromones(double[][] pheromones, int[][] ants, int[][] dists)
        {
            for (int i = 0; i < pheromones.Length; ++i)
            {
                for (int j = i + 1; j < pheromones[i].Length; ++j)
                {
                    for (int k = 0; k < ants.Length; ++k)
                    {
                        double length = Length(ants[k], dists); //длина пути конкретного муравья
                        double decrease = (1.0 - rho) * pheromones[i][j];
                        double increase = 0.0;
                        if (EdgeInTrail(i, j, ants[k]) == true)
                            increase = (Q / length);
                        pheromones[i][j] = decrease + increase;
                        if (pheromones[i][j] < 0.0001)
                            pheromones[i][j] = 0.0001;
                        else if (pheromones[i][j] > 100000.0)
                            pheromones[i][j] = 100000.0;
                        pheromones[j][i] = pheromones[i][j];
                    }
                }
            }
        }

        private static bool EdgeInTrail(int cityX, int cityY, int[] trail) //сиежные ли города в пути?
        {
            int lastIndex = trail.Length - 1;
            int idx = IndexOfTarget(trail, cityX);

            if (idx == 0 && trail[1] == cityY)
                return true;
            else
            if (idx == 0 && trail[lastIndex] == cityY)
                return true;
            else
            if (idx == 0)
                return false;
            else
            if (idx == lastIndex && trail[lastIndex - 1] == cityY)
                return true;
            else
            if (idx == lastIndex && trail[0] == cityY)
                return true;
            else
            if (idx == lastIndex)
                return false;
            else
            if (trail[idx - 1] == cityY)
                return true;
            else
            if (trail[idx + 1] == cityY)
                return true;
            else
                return false;
        }
        private static int[][] MakeGraphDistances(string name, ref int numCities)
        {
            //открываем файл для чтения
            StreamReader FileReader;
            try
            {
                FileReader = new StreamReader(name);
            }
            catch (Exception error)
            {
                Console.WriteLine(error.Message);
                throw new Exception("Bad open file");
            }
            //считываем количество городов
            string info;
            info = FileReader.ReadLine();
            try
            {
                numCities = Int32.Parse(info);
            }
            catch (Exception)
            {
                throw new Exception("Bad File");
            }
            //иниицализируем матрицу смежности
            int[][] dists = new int[numCities][];
            for (int i = 0; i < dists.Length; ++i)
            {
                dists[i] = new int[numCities];
                for (int j = 0; j < dists.Length; ++j)
                    dists[i][j] = Int32.MaxValue;
            }
            //последовательно из файла вводим рёбра
            while (!FileReader.EndOfStream)
            {
                info = FileReader.ReadLine();
                string[] temp = new string[3];
                int cur = 0;
                if (info.Length < 1)
                    break;
                for (int i = 0; i < 3 && cur < info.Length; i++)
                {
                    while (info[cur] != ' ')
                    {
                        temp[i] += info[cur];
                        cur++;
                        if (cur == info.Length)
                            break;
                    }
                    cur++;
                }
                dists[Int32.Parse(temp[0]) - 1][Int32.Parse(temp[1]) - 1] = Int32.Parse(temp[2]);
            }
            return dists;
        }
        private static double Distance(int cityX, int cityY, int[][] dists)
        {
            return dists[cityX][cityY];
        }
        private static void Display(int[] trail)
        {
            for (int i = 0; i < trail.Length; ++i)
            {
                Console.Write((trail[i] + 1) + " ");
                if (i > 0 && i % 20 == 0) Console.WriteLine("");
            }
            Console.WriteLine("");
        }
        private static void ShowAnts(int[][] ants, int[][] dists)
        {
            for (int i = 0; i < ants.Length; ++i)
            {
                Console.Write(i + ": [ ");

                for (int j = 0; j < ants[i].Length; ++j)
                    Console.Write((ants[i][j] + 1) + " ");
                Console.Write("] len = ");
                double len = Length(ants[i], dists);
                Console.Write(len.ToString("F1"));
                Console.WriteLine("");
            }
        }
    }
}
