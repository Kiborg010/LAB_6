using System.Text;
using System.Text.RegularExpressions;

class Program
{
    static char[] arraySignOne = new char[] { '.', '!', '?' }; //Массив со знаками, которые должны находиться в конце предложения
    static char[] arraySignTwo = new char[] { ',', ';', ':' };//Массив со знаками, которые должны находиться внутри предложения
    static char[] arraySignOneAndTwo = arraySignOne.Concat(arraySignTwo).ToArray(); //Массив со всеми знаками
    static int Input(int left, int right) // Функция для проверки числового ввода
    {
        int n;
        bool nIsCorrected;
        do //Программа будет запрашивать число до тех пор, пока оно не станет входить в указанные границы необъодимого диапазона
        {
            nIsCorrected = int.TryParse(Console.ReadLine(), out n);
            if (!nIsCorrected || !((left <= n) && (n <= right)))
            {
                Console.WriteLine($"Ошибка ввода. Вам необходимо ввести целое число от {left} до {right}");
            };
        } while (!nIsCorrected || !((left <= n) && (n <= right)));
        return n;
    }

    static void PrintCommands(int typePrint = 0) //Функция для вывода доступных команд
    {
        if (typePrint == 0) //В зависимости от выбранного типа выводится нужное сообщение
        {
            Console.WriteLine("1. Ввести строку");
            Console.WriteLine("2. Использовать заранее сформированный массив");
            Console.WriteLine("3. Выполнить необходимую обработку строки (вариант 17)");
            Console.WriteLine("4. Напечатать строку");
            Console.WriteLine("5. Завершить работу");
        }
        else if (typePrint == 1)
        {
            Console.Write("Введите команду: ");
        }
        else if (typePrint == 2)
        {
            Console.WriteLine("Введите строку: ");
        }
        else if (typePrint == 3)
        {
            Console.Write("Введите номер строки: ");
        }
    }

    static string[] GetSentence(string phrase) //Функция для разделения строки на предложения
    {
        phrase = " " + phrase;
        Regex regex = new Regex("(([^.!?]+)([.!?]))");
        MatchCollection matches = regex.Matches(phrase);
        string[] array = new string[0];
        foreach (Match match in matches)
        {
            string str = match.ToString().Remove(0, 1);
            Array.Resize(ref array, array.Length + 1);
            array[array.Length - 1] = str;
        };
        return array; //Возвращение массива с предложениями, полученными из строки
    }

    static bool CheckStringIsCorrect(string inputString) //Функция для проверки корректности строки для обработки
    {
        if (inputString != "")
        {
            string stringCheck = DoCorrect(inputString);
            if ((stringCheck.Length == 1) && (arraySignOneAndTwo.Contains(char.Parse(stringCheck))))
            {
                return false;
            }
            Regex regex = new Regex("^[a-zA-Zа-яА-ЯёЁ0-9,.;:?! ]*$"); //Проверка на корректность введённых символов. Могут быть только буквы и цифры
            if (regex.IsMatch(inputString)) 
            {
                if (!arraySignOne.Contains(inputString[^1]))//Проверка на то, что строка оканчивается на .!?
                {
                    return false;
                }
                foreach (char el in arraySignOneAndTwo) //Проверка на то, что знак препинания стоит верно. То есть нет случаев типа "a,б" или "a.б" и т.п.
                {
                    string str = "\\" + el;
                    regex = new Regex(@$"\w{str}\w");
                    if (regex.IsMatch(inputString))
                    {
                        return false;
                    };
                }
                foreach (char el1 in arraySignOneAndTwo) //Проверка на то, что знаки препинания не стоят рядом. То есть нет случаев типа ",." или ":!" и т.п.
                                                         //Случаи типа "abc,,,," или "aбс....." программой обрабатываются в другом месте и лишние знаки убираются.
                {
                    foreach (char el2 in arraySignOneAndTwo)
                    {
                        string str = "\\" + el1 + "\\" + el2;
                        regex = new Regex(@$"{str}");
                        if (regex.IsMatch(inputString))
                        {
                            return false;
                        };
                    }
                }
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            return false;
        }
    }

    static string DelSpaces(string inputString) //Функция для удаления лишних пробелов
    {
        Regex regex = new Regex(@"\s+");
        inputString = regex.Replace(inputString, " ");
        return inputString;
    }
    static string DoCorrect(string inputString) //Функция для корректировки предложения
    {
        if (inputString != "")
        {
            inputString = inputString.ToLower(); //Первую букву сделать заглавной
            string letter = inputString[0].ToString().ToUpper();
            inputString = inputString.Remove(0, 1).Insert(0, letter);
            inputString = DelSpaces(inputString);
            foreach (char el in arraySignOneAndTwo) //Переделывание случаев типа "a , b" в "a, b"
            {
                string str = " \\" + el;
                inputString = Regex.Replace(inputString, @$"({str})+", el.ToString());
            }
            foreach (char el in arraySignOneAndTwo) //Удаление дублирующихся знаков препинания
            {
                string str = "\\" + el + "\\" + el;
                inputString = Regex.Replace(inputString, @$"({str})+", el.ToString());
            }
            foreach (char el in arraySignOneAndTwo)//Тоже удаление дублирующихся знаков препинания
            {
                string str = "\\" + el + "\\" + el;
                inputString = Regex.Replace(inputString, @$"({str})+", el.ToString());
            }
        }
        return inputString;
    }

    static string DoCorrectForPrint(string inputString) //Функция для формирования массива с корректными предложениями для печати
    {
        string result = "";
        string[] array = GetSentence(inputString);
        foreach(string el in array)
        {
            result += DoCorrect(el) + " ";
        }
        return result;
    }

    static string ChangeWords(string sentence) //Функция для выполнения основного задания
    {
        string result = "";
        Regex regex = new Regex(@"\S+");
        MatchCollection matches = regex.Matches(sentence); //Получаем слова предложения
        int count = 0;
        foreach (Match match in matches)
        {
            string str = match.Value.ToString();
            char lastSign = str[^1];
            if (arraySignOneAndTwo.Contains(lastSign)) //Если в конце слова стоит знак препинания, то его временно убираем, чтобы он циклически не сдвинулся
            {
                str = str.Substring(0, str.Length - 1);
            }
            else
            {
                lastSign = '\0';
            }
            count++;
            int index = count;
            char[] arraySymbols = str.ToString().ToArray();
            string[] newArraySymbols = new string[str.Length];
            for (int i = 0; i < arraySymbols.Length; i++) //Делаем циклический сдвиг
            {
                int newIndex = i - index;
                if (newIndex < 0)
                {
                    newIndex = newIndex + (int)Math.Ceiling((double)Math.Abs(newIndex) / arraySymbols.Length) * arraySymbols.Length;
                };
                newArraySymbols[newIndex] = arraySymbols[i].ToString();
            };
            result = result + string.Join("", newArraySymbols) + lastSign + " ";//Добавляем слова к новому предложению
        }
        return result;
    }

    static void Main()
    {
        int numberAnswer = 0;
        string text = "";
        string inputString = "";
        string[] arrayStrings = File.ReadAllLines(@"../../../text.txt"); //Массив формируется из строк в файле
        while (numberAnswer != 5)
        {
            Console.Clear();
            if (text != "")
            {
                Console.WriteLine(text);
            }
            PrintCommands(0);
            PrintCommands(1);
            numberAnswer = Input(1, 5);
            switch (numberAnswer)
            {
                case 1:
                    {
                        Console.Clear();
                        bool inputStringIsCorrect;
                        do
                        {
                            PrintCommands(2);
                            inputString = DoCorrect(Console.ReadLine());
                            inputStringIsCorrect = CheckStringIsCorrect(inputString);
                            if (!inputStringIsCorrect)
                            {
                                Console.Clear();
                                Console.WriteLine("Ошибка ввода. Вам необходимо ввести строку, которая содержит слова, знаки препинания и пробелы произвольного количества");
                            }
                        }
                        while (!inputStringIsCorrect);
                        text = "Была проведена корректировка пробелов, заглавных букв и знаков препинания";
                        break;
                    };
                case 2:
                    {
                        Console.Clear();
                        if (arrayStrings.Length != 0)
                        {
                            string localText = "";
                            bool inputStringIsCorrect;
                            do
                            {
                                Console.Clear();
                                if (localText != "")
                                {
                                    Console.WriteLine(localText);
                                };
                                Console.WriteLine("Массив: ");
                                for (int i = 0; i < arrayStrings.Length; i++)
                                {
                                    Console.WriteLine($"{i + 1}. " + arrayStrings[i]);
                                };
                                PrintCommands(3);
                                inputString = DoCorrect(arrayStrings[Input(1, arrayStrings.Length) - 1]);
                                inputStringIsCorrect = CheckStringIsCorrect(inputString);
                                localText = "Выбранная строка некорректна";
                                Console.Clear();
                            }
                            while (!inputStringIsCorrect);
                            text = "Строка выбрана. Была проведена корректировка пробелов, заглавных букв и знаков препинания";
                        }
                        else
                        {
                            text = "На данный момент в массиве нет строк";
                        }
                        break;
                    };
                case 3:
                    {
                        if (inputString == "")
                        {
                            text = "Строка пуста";
                        }
                        else
                        {
                            text = "Исходная строка:" + "\n" + DoCorrectForPrint(inputString) + "\n\n" + "Результат обработки:" + "\n";
                            string[] array = GetSentence(inputString);
                            string result = "";
                            foreach (string el in array)
                            {
                                Console.WriteLine(DoCorrect(ChangeWords(el)));
                                result = result + DoCorrect(ChangeWords(el)) + " ";
                            }

                            text += DelSpaces(result) + "\n";
                        }
                        Console.Clear();
                        break;
                    };
                case 4:
                    {
                        if (inputString == "")
                        {
                            text = "Строка пуста";
                        }
                        else
                        {
                            text = "Исходная строка:" + "\n" + DoCorrectForPrint(inputString) + "\n";
                        }
                        Console.Clear();
                        break;
                    };
                case 5:
                    {
                        Console.Clear();
                        Console.WriteLine("Завершаем работу");
                        break;
                    }
                default:
                    {
                        Console.WriteLine("Ошибка ввода");
                        break;
                    }
            }
        }
    }
}