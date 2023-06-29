using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace LabCalc
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 

    // математическое действие
    abstract class Act
    {
        public abstract decimal Exec(decimal x, decimal y); // произвести операцию
        abstract public char Char { get; } // символ действия
    }
    // плюс
    class Act_Plus : Act
    {
        private char _char = '+';
        public override char Char { get { return _char; } }
        override public decimal Exec(decimal x, decimal y)
        {
            return x + y;
        }
    }
    // минус
    class Act_Minus : Act
    {
        private char _char = '-';
        public override char Char { get { return _char; } }
        override public decimal Exec(decimal x, decimal y)
        {
            return x - y;
        }
    }
    // умножение
    class Act_Mult : Act
    {
        private char _char = '*';
        public override char Char { get { return _char; } }
        override public decimal Exec(decimal x, decimal y)
        {
            return x * y;
        }
    }
    // деление
    class Act_Div : Act
    {
        private char _char = '/';
        public override char Char { get { return _char; } }
        override public decimal Exec(decimal x, decimal y)
        {
            return x / y;
        }
    }
    // взятие процента от числа
    class Act_Percent : Act
    {
        private char _char = '%';
        public override char Char { get { return _char; } }
        override public decimal Exec(decimal x, decimal y)
        {
            return y / 100 * x;
        }
    }

    public partial class MainWindow : Window
    {
        private decimal primary = 0; // главное число (первичное)
        private decimal secondary = 0; // второе число (вторичное)
        private decimal memory = 0; // число в памяти
        private Act act = null; // выбранное математическое действие

        private int pointPosition = 0;

        public MainWindow()
        {
            InitializeComponent();
            btn_1.Click += Btn_1_Click;
            btn_2.Click += Btn_2_Click;
            btn_3.Click += Btn_3_Click;
            btn_4.Click += Btn_4_Click;
            btn_5.Click += Btn_5_Click;
            btn_6.Click += Btn_6_Click;
            btn_7.Click += Btn_7_Click;
            btn_8.Click += Btn_8_Click;
            btn_9.Click += Btn_9_Click;
            btn_0.Click += Btn_0_Click;
            btn_delete.Click += Btn_delete_Click;
            btn_plusminus.Click += Btn_plusminus_Click;
            btn_mc.Click += Btn_mc_Click;
            btn_mr.Click += Btn_mr_Click;
            btn_ms.Click += Btn_ms_Click;
            btn_mplus.Click += Btn_mplus_Click;
            btn_mminus.Click += Btn_mminus_Click;
            btn_sqrt.Click += Btn_sqrt_Click;
            btn_div1x.Click += Btn_div1x_Click;
            btn_percent.Click += Btn_percent_Click;
            btn_plus.Click += Btn_plus_Click;
            btn_minus.Click += Btn_minus_Click;
            btn_mult.Click += Btn_mult_Click;
            btn_div.Click += Btn_div_Click;
            btn_ok.Click += Btn_ok_Click;
            btn_c.Click += Btn_c_Click;
            btn_point.Click += Btn_point_Click;


            this.KeyDown += new KeyEventHandler(MainWindow_KeyDown);
        }

        // событие нажатия на кнопку клавиатуры
        void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.Key)
            {
                case Key.Divide: SetAct(new Act_Div()); break;
                case Key.Multiply: SetAct(new Act_Mult()); break;
                case Key.Subtract: SetAct(new Act_Minus()); break;
                case Key.Add: SetAct(new Act_Plus()); break;

                case Key.D0: AddDigit(0); break;
                case Key.D1: AddDigit(1); break;
                case Key.D2: AddDigit(2); break;
                case Key.D3: AddDigit(3); break;
                case Key.D4: AddDigit(4); break;
                case Key.D5: AddDigit(5); break;
                case Key.D6: AddDigit(6); break;
                case Key.D7: AddDigit(7); break;
                case Key.D8: AddDigit(8); break;
                case Key.D9: AddDigit(9); break;

                case Key.NumPad0: AddDigit(0); break;
                case Key.NumPad1: AddDigit(1); break;
                case Key.NumPad2: AddDigit(2); break;
                case Key.NumPad3: AddDigit(3); break;
                case Key.NumPad4: AddDigit(4); break;
                case Key.NumPad5: AddDigit(5); break;
                case Key.NumPad6: AddDigit(6); break;
                case Key.NumPad7: AddDigit(7); break;
                case Key.NumPad8: AddDigit(8); break;
                case Key.NumPad9: AddDigit(9); break;

                case Key.Delete: Clear(); break;
                case Key.Back: RemoveDigit(); break;
                case Key.Enter: Calc(); break;
                case Key.OemComma: AddPoint(); break;
            }
        }

        // обновление текста в приложении
        private void refresh(string str = null)
        {
            // главный дисплей
            if (str == null) display_primary.Content = primary.ToString();
            else display_primary.Content = str;
            // вторичный дисплей
            string tempSecondary = secondary.ToString();
            if (act != null) tempSecondary += " " + act.Char.ToString();
            display_secondary.Content = tempSecondary;
        }
        // добавление указанной цифры в конец текущего числа
        private void AddDigit(int digit)
        {
            if (digit >= 0 && digit <= 9)
            {
                if (pointPosition != 0) // если дробное
                {
                    if (pointPosition >= 10) return; // ограничение
                    int dec = (int)Math.Pow(10, pointPosition); // вычисление разряда
                    decimal p = (decimal)digit / dec;
                    primary = primary + (p); // добавление цифры
                    pointPosition++;
                }
                else // число не дробное
                {
                    if (Math.Abs(primary) * 10 + digit < decimal.MaxValue) // проверка на превышение макс. значения
                    {
                        decimal prev = primary; // предыдущее значение
                        primary = Math.Abs(primary) * 10 + digit; // добавление цифры
                        if (prev < 0) primary = -primary; // если число было отрицательным, сделать его снова отрицательным
                    }
                }
                refresh();
            }
        }
        // удаление цифры с конца числа
        private void RemoveDigit()
        {
            if (pointPosition > 2) // если число дробное
            {
                int dec = (int)Math.Pow(10, pointPosition-2);
                //decimal toMinus = (primary - Math.Truncate(primary)) * (dec);
                //toMinus = (toMinus - Math.Truncate(toMinus)) / dec;
                primary = Math.Floor(primary * dec) / (dec); // округляем число
                if (pointPosition == 3) pointPosition = 0;
                else pointPosition--;
            }
            else // если число не дробное
            {
                primary = Math.Floor(primary / 10);
            }
            refresh();
        }
        // поставить заданное матем. действие
        private void SetAct(Act _act)
        {
            act = _act;
            if (secondary == 0) // если вторичного числа нет, то на его место ставим первичное
            {
                secondary = primary;
                primary = 0;
                pointPosition = 0;
            }
            refresh();
        }
        // очистка
        private void Clear()
        {
            act = null;
            secondary = 0;
            primary = 0;
            pointPosition = 0;
            refresh();
        }
        // рассчёт
        private void Calc()
        {
            if (act != null) // если поставлена операция
            {
                try
                {
                    //decimal temp = primary;
                    primary = act.Exec(secondary, primary);
                    pointPosition = 0;
                    secondary = primary;
                }
                catch
                {
                    refresh("ошибка");
                }
            }
            refresh();
            primary = 0;
        }
        // добавить запятую
        private void AddPoint()
        {
            pointPosition = 1; // primary.ToString().Length;
            refresh();
        }

        // добавление точки
        private void Btn_point_Click(object sender, RoutedEventArgs e)
        {
            AddPoint();
        }
        // очистка
        private void Btn_c_Click(object sender, RoutedEventArgs e)
        {
            Clear();
        }
        // рассчёт
        private void Btn_ok_Click(object sender, RoutedEventArgs e)
        {
            Calc();
        }
        // выбор процента
        private void Btn_percent_Click(object sender, RoutedEventArgs e)
        {
            SetAct(new Act_Percent());
        }
        // выбор деления
        private void Btn_div_Click(object sender, RoutedEventArgs e)
        {
            SetAct(new Act_Div());
        }
        // выбор умножения
        private void Btn_mult_Click(object sender, RoutedEventArgs e)
        {
            SetAct(new Act_Mult());
        }
        // выбор минуса
        private void Btn_minus_Click(object sender, RoutedEventArgs e)
        {
            SetAct(new Act_Minus());
        }
        // выбор плюса
        private void Btn_plus_Click(object sender, RoutedEventArgs e)
        {
            SetAct(new Act_Plus());
        }

        // квадратный корень
        private void Btn_sqrt_Click(object sender, RoutedEventArgs e)
        {
            if (primary >= 0)
            {
                try
                {
                    primary = (decimal)Math.Sqrt((double)primary);
                }
                catch
                {

                }
            }
            refresh();
        }
        // 1 делить на x
        private void Btn_div1x_Click(object sender, RoutedEventArgs e)
        {
            primary = 1 / primary;
            refresh();
        }

        // отнятие от памяти
        private void Btn_mminus_Click(object sender, RoutedEventArgs e)
        {
            memory -= primary;
            refresh();
        }
        // прибавление к памяти
        private void Btn_mplus_Click(object sender, RoutedEventArgs e)
        {
            memory += primary;
            refresh();
        }
        // запись в память
        private void Btn_ms_Click(object sender, RoutedEventArgs e)
        {
            memory = primary;
            refresh();
        }
         // считывание с памяти
        private void Btn_mr_Click(object sender, RoutedEventArgs e)
        {
            primary = memory;
            refresh();
        }
        // очистка памяти
        private void Btn_mc_Click(object sender, RoutedEventArgs e)
        {
            memory = 0;
            refresh();
        }

        // изменение знака с плюса на минус и обратно
        private void Btn_plusminus_Click(object sender, RoutedEventArgs e)
        {
            primary = -primary;
            refresh();
        }
        // событие нажатия на кнопку <- (delete)
        private void Btn_delete_Click(object sender, RoutedEventArgs e)
        {
            RemoveDigit();
            refresh();
        }
        // события нажатия на кнопку цифры
        private void Btn_1_Click(object sender, RoutedEventArgs e)
        {
            AddDigit(1);
        }
        private void Btn_2_Click(object sender, RoutedEventArgs e)
        {
            AddDigit(2);
        }
        private void Btn_3_Click(object sender, RoutedEventArgs e)
        {
            AddDigit(3);
        }
        private void Btn_4_Click(object sender, RoutedEventArgs e)
        {
            AddDigit(4);
        }
        private void Btn_5_Click(object sender, RoutedEventArgs e)
        {
            AddDigit(5);
        }
        private void Btn_6_Click(object sender, RoutedEventArgs e)
        {
            AddDigit(6);
        }
        private void Btn_7_Click(object sender, RoutedEventArgs e)
        {
            AddDigit(7);
        }
        private void Btn_8_Click(object sender, RoutedEventArgs e)
        {
            AddDigit(8);
        }
        private void Btn_9_Click(object sender, RoutedEventArgs e)
        {
            AddDigit(9);
        }
        private void Btn_0_Click(object sender, RoutedEventArgs e)
        {
            AddDigit(0);
        }
    }
}
