using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace PraktGenericsNullable
{
    internal static class Program
    {
        private static void Main()
        {
            Console.OutputEncoding = Encoding.UTF8;
            CultureInfo.CurrentCulture = CultureInfo.InvariantCulture;

            RunTask1_1();
            Console.WriteLine(new string('=', 60));
            RunTask1_2();
            Console.WriteLine(new string('=', 60));
            RunTask2_1();
            Console.WriteLine(new string('=', 60));
            RunTask3_1();

           
        }

        private static void RunTask1_1()
        {
            Console.WriteLine("=== ЗАВДАННЯ 1.1: Узагальнені методи GenericUtils ===");

            // Тест Swap (виведення типів автоматичне)
            int x = 10, y = 20;
            GenericUtils.Swap(ref x, ref y);
            Console.WriteLine($"Swap (int): x={x}, y={y}");

            string s1 = "Привіт", s2 = "Світ";
            GenericUtils.Swap(ref s1, ref s2);
            Console.WriteLine($"Swap (string): s1={s1}, s2={s2}");

            // Тест FindMin
            int[] numbers = { 5, 3, 8, 1, 9, 2 };
            Console.WriteLine($"Min (int[]): {GenericUtils.FindMin(numbers)}");

            string[] words = { "банан", "апельсин", "яблуко" };
            Console.WriteLine($"Min (string[]): {GenericUtils.FindMin(words)}");

            // Перевірка винятку на порожньому масиві
            try
            {
                GenericUtils.FindMin(Array.Empty<int>());
            }
            catch (ArgumentException ex)
            {
                Console.WriteLine($"Очікуваний виняток FindMin: {ex.Message}");
            }

            // Тест Filter
            int[] evens = GenericUtils.Filter(numbers, n => n % 2 == 0);
            Console.WriteLine($"Парні: {string.Join(", ", evens)}");

            int[] none = GenericUtils.Filter(numbers, n => n > 100);
            Console.WriteLine($"Елементи > 100 (порожній масив): довжина = {none.Length}");
        }
        private static void RunTask1_2()
        {
            Console.WriteLine("=== ЗАВДАННЯ 1.2: Обмеження параметрів типу (where) ===");

            var student = ConstrainedUtils.CreateAndDisplay<Student>();
            Console.WriteLine($"Тип створеного об'єкта: {student.GetType().Name}");

            var numbers = new List<int> { 5, 2, 8, 1, 9 };
            ConstrainedUtils.CompareAndSort(numbers);

            var names = new List<string> { "Марія", "Андрій", "Олена" };
            ConstrainedUtils.CompareAndSort(names);

            // Демонстрація комбінованого обмеження where T : class, IComparable<T>, new()
            var groupList = new List<Student>
    {
        new Student { Name = "Олена", Grade = 95 },
        new Student { Name = "Богдан", Grade = 82 },
        new Student { Name = "Ірина", Grade = 98 }
    };
            ConstrainedUtils.CreateSortAndDisplay(groupList);
        }
        private static void RunTask2_1()
        {
            Console.WriteLine("=== ЗАВДАННЯ 2.1: Узагальнена черга SimpleQueue<T> ===");

            // 1. Черга цілих чисел (int) + перевірка автоматичного розширення (Resize)
            var intQueue = new SimpleQueue<int>(2);
            intQueue.Enqueue(10);
            intQueue.Enqueue(20);
            intQueue.Enqueue(30); // викличе Resize()

            Console.WriteLine($"Count (int): {intQueue.Count}");
            Console.WriteLine($"Peek: {intQueue.Peek()}");
            Console.WriteLine($"Dequeue: {intQueue.Dequeue()}");
            Console.WriteLine($"Dequeue: {intQueue.Dequeue()}");
            Console.WriteLine($"Count після вилучень: {intQueue.Count}");

            // 2. Черга рядків (string)
            var strQueue = new SimpleQueue<string>();
            strQueue.Enqueue("Перший");
            strQueue.Enqueue("Другий");
            strQueue.Enqueue("Третій");
            strQueue.PrintAll();

            // 3. Черга власного класу (Student)
            var studentQueue = new SimpleQueue<Student>();
            studentQueue.Enqueue(new Student { Name = "Данило", Grade = 96 });
            studentQueue.Enqueue(new Student { Name = "Марія", Grade = 92 });
            studentQueue.PrintAll();

            // 4. Виняток при Dequeue з порожньої черги
            var emptyQueue = new SimpleQueue<int>();
            try
            {
                emptyQueue.Dequeue();
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"Очікуваний виняток порожньої черги: {ex.Message}");
            }
        }
        private static void RunTask3_1()
        {
            Console.WriteLine("=== ЗАВДАННЯ 3.1: Nullable<T> та колекції ===");

            var students = new List<StudentRecord>
    {
        new StudentRecord { Name = "Олена",    MathGrade = 90,   PhysicsGrade = 85,   ProgrammingGrade = 95 },
        new StudentRecord { Name = "Андрій",   MathGrade = 75,   PhysicsGrade = null, ProgrammingGrade = 80 },
        new StudentRecord { Name = "Марія",    MathGrade = null, PhysicsGrade = 92,   ProgrammingGrade = 88 },
        new StudentRecord { Name = "Петро",    MathGrade = 60,   PhysicsGrade = 70,   ProgrammingGrade = null },
        new StudentRecord { Name = "Катерина", MathGrade = null, PhysicsGrade = null, ProgrammingGrade = 95 }
    };

            GradeAnalyzer.PrintReport(students);

            Console.WriteLine("\nСередні бали по предметах:");
            var averages = GradeAnalyzer.GetSubjectAverages(students);
            foreach (var kvp in averages)
                Console.WriteLine($"  {kvp.Key}: {kvp.Value:F1}");
        }
    }

    // ================= ЗАВДАННЯ 1.1 =================
    public static class GenericUtils
    {
        public static void Swap<T>(ref T a, ref T b)
        {
            T temp = a;
            a = b;
            b = temp;
        }

        public static T FindMin<T>(T[] array) where T : IComparable<T>
        {
            if (array == null || array.Length == 0)
                throw new ArgumentException("Масив не може бути порожнім або null.", nameof(array));

            T min = array[0];
            for (int i = 1; i < array.Length; i++)
            {
                if (array[i].CompareTo(min) < 0)
                    min = array[i];
            }
            return min;
        }

        public static T[] Filter<T>(T[] array, Func<T, bool> predicate)
        {
            if (array == null) throw new ArgumentNullException(nameof(array));
            if (predicate == null) throw new ArgumentNullException(nameof(predicate));

            var result = new List<T>();
            foreach (T item in array)
            {
                if (predicate(item))
                    result.Add(item);
            }
            return result.ToArray();
        }
    }
    // ================= ЗАВДАННЯ 1.2 =================
    public static class ConstrainedUtils
    {
        public static T CreateAndDisplay<T>() where T : new()
        {
            T instance = new T();
            Console.WriteLine($"Створено екземпляр: {instance}");
            return instance;
        }

        public static void CompareAndSort<T>(List<T> items) where T : IComparable<T>
        {
            items.Sort();
            Console.WriteLine($"Відсортовано: {string.Join(", ", items)}");
        }

        // Комбінування кількох обмежень одночасно
        public static void CreateSortAndDisplay<T>(List<T> items) where T : class, IComparable<T>, new()
        {
            T defaultItem = new T();
            items.Add(defaultItem);
            items.Sort();
            Console.WriteLine("Після додавання default-елемента та сортування:");
            foreach (var item in items)
                Console.WriteLine($"  {item}");
        }

        // Приклад, який НЕ компілюється без обмеження new():
        // public static T BrokenCreate<T>() => new T(); // Помилка CS0304
    }

    public class Student : IComparable<Student>
    {
        public string Name { get; set; } = "Невідомий";
        public int Grade { get; set; } = 0;

        public int CompareTo(Student? other)
        {
            if (other == null) return 1;
            return Grade.CompareTo(other.Grade);
        }

        public override string ToString() => $"Student: {Name}, Grade: {Grade}";
    }
    // ================= ЗАВДАННЯ 2.1 =================
    public class SimpleQueue<T>
    {
        private T[] _items;
        private int _head;
        private int _tail;
        private int _count;

        public SimpleQueue(int capacity = 8)
        {
            if (capacity <= 0)
                throw new ArgumentOutOfRangeException(nameof(capacity), "Місткість має бути більшою за 0.");
            _items = new T[capacity];
            _head = 0;
            _tail = 0;
            _count = 0;
        }

        public int Count => _count;
        public bool IsEmpty => _count == 0;

        public void Enqueue(T item)
        {
            if (_count == _items.Length)
                Resize();

            _items[_tail] = item;
            _tail = (_tail + 1) % _items.Length;
            _count++;
        }

        public T Dequeue()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Черга порожня: неможливо вилучити елемент.");

            T value = _items[_head];
            _items[_head] = default!; // очищення комірки
            _head = (_head + 1) % _items.Length;
            _count--;
            return value;
        }

        public T Peek()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Черга порожня: немає елементів для перегляду.");

            return _items[_head];
        }

        private void Resize()
        {
            int newCapacity = _items.Length * 2;
            T[] newArray = new T[newCapacity];

            for (int i = 0; i < _count; i++)
            {
                newArray[i] = _items[(_head + i) % _items.Length];
            }

            _items = newArray;
            _head = 0;
            _tail = _count;
        }

        public void PrintAll()
        {
            var elements = new List<string>(_count);
            for (int i = 0; i < _count; i++)
            {
                T item = _items[(_head + i) % _items.Length];
                elements.Add(item?.ToString() ?? "null");
            }
            Console.WriteLine($"Вміст черги ({typeof(T).Name}): {string.Join(", ", elements)}");
        }
    }
    // ================= ЗАВДАННЯ 3.1 =================
    public class StudentRecord
    {
        public string Name { get; set; } = string.Empty;
        public int? MathGrade { get; set; }
        public int? PhysicsGrade { get; set; }
        public int? ProgrammingGrade { get; set; }
    }

    public static class GradeAnalyzer
    {
        public static double? CalculateAverage(StudentRecord? student)
        {
            if (student == null) return null;

            int sum = 0;
            int count = 0;

            if (student.MathGrade.HasValue)
            {
                sum += student.MathGrade.Value;
                count++;
            }
            if (student.PhysicsGrade.HasValue)
            {
                sum += student.PhysicsGrade.Value;
                count++;
            }
            if (student.ProgrammingGrade.HasValue)
            {
                sum += student.ProgrammingGrade.Value;
                count++;
            }

            return count > 0 ? (double)sum / count : null;
        }

        public static void PrintReport(List<StudentRecord> students)
        {
            Console.WriteLine($"{"Ім'я",-15} {"Математика",12} {"Фізика",10} {"Програм.",10} {"Середній",10}");
            Console.WriteLine(new string('-', 60));

            foreach (var student in students)
            {
                string mathText = student?.MathGrade?.ToString() ?? "н/а";
                string physText = student?.PhysicsGrade?.ToString() ?? "н/а";
                string progText = student?.ProgrammingGrade?.ToString() ?? "н/а";

                double? avg = CalculateAverage(student);
                string avgText = avg?.ToString("F1") ?? "н/а";

                Console.WriteLine($"{student?.Name ?? "Невідомий",-15} {mathText,12} {physText,10} {progText,10} {avgText,10}");
            }
        }

        public static Dictionary<string, double> GetSubjectAverages(List<StudentRecord> students)
        {
            Dictionary<string, List<int>>? groupedScores = null;
            groupedScores ??= new Dictionary<string, List<int>>();

            foreach (var s in students)
            {
                if (s.MathGrade.HasValue)
                {
                    if (!groupedScores.ContainsKey("Математика"))
                        groupedScores["Математика"] = new List<int>();
                    groupedScores["Математика"].Add(s.MathGrade.GetValueOrDefault());
                }
                if (s.PhysicsGrade.HasValue)
                {
                    if (!groupedScores.ContainsKey("Фізика"))
                        groupedScores["Фізика"] = new List<int>();
                    groupedScores["Фізика"].Add(s.PhysicsGrade.GetValueOrDefault());
                }
                if (s.ProgrammingGrade.HasValue)
                {
                    if (!groupedScores.ContainsKey("Програмування"))
                        groupedScores["Програмування"] = new List<int>();
                    groupedScores["Програмування"].Add(s.ProgrammingGrade.GetValueOrDefault());
                }
            }

            Dictionary<string, double>? result = null;
            result ??= new Dictionary<string, double>();

            foreach (var pair in groupedScores)
            {
                result[pair.Key] = pair.Value.Count > 0 ? pair.Value.Average() : 0.0;
            }

            return result;
        }
    }    // Нижче перед останньою закриваючою дужкою '}' додаватимемо класи наступних завдань
}