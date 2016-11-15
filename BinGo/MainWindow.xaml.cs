using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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

namespace BINGO
{
    /// <summary>
    /// MainWindow.xaml 的互動邏輯
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void BTN_OK_Click(Object sender, RoutedEventArgs e)
        {
            Result = GetRand();
            if (Result > 75 || Result <= 0)
            {
                MessageBox.Show("號碼錯誤");
                return;
            }
            /*
            while (data.Contains(result) && data.Count < 75)
            {
                result = GetRand();
            }
            */
            if (!data.Contains(Result))
            {
                data.Add(Result);
                if (Source.Contains(Result))
                    Source.Remove(Result);
                else
                    MessageBox.Show("ERROR");
                if (!sortdata.ContainsKey(Result / 10))
                {
                    sortdata[Result / 10] = new List<int>();
                }
                if (Result % 10 == 0)
                {
                    if (!sortdata.ContainsKey((Result / 10) - 1))
                    {
                        sortdata[(Result / 10) - 1] = new List<int>();
                    }
                    sortdata[(Result / 10) - 1].Add(Result);
                }
                else
                    sortdata[Result / 10].Add(Result);
                sortdata[Result / 10].Sort();
            }
            else
            {
                if (TB_INPUT.IsEnabled)
                {
                    MessageBox.Show("號碼重複");
                }
                return;
            }

            ShowResult();

            if (data.Count == 75)
            {
                MessageBox.Show("遊戲結束", "結束", MessageBoxButton.OK, MessageBoxImage.Information);
                ReStart();
            }

        }

        private void ShowResult()
        {
            if (data.Count <= 1)
            {
                TB_R.Text = String.Empty;
                if (data.Count == 0)
                    return;
            }
            int _result = data[data.Count - 1];
            label.Content = _result.ToString();
            LB_Count.Content = data.Count.ToString();

            TB_R.Text += (_result < 10 ? " " : "") + _result.ToString() + (data.Count == 75 ? "" : "  ") + (data.Count % 12 == 0 ? "\r\n" : "");

            foreach (var x in sortdata)
            {
                if (x.Key == 0)
                {
                    string ris = String.Empty;
                    for (int i = 0; i < x.Value.Count; i++)
                    {
                        ris += (x.Value[i] == 10 ? "" : " ") + x.Value[i].ToString() + (i == x.Value.Count - 1 ? "" : "  ");
                    }
                    LB_Result[x.Key].Content = ris;
                }
                else
                    LB_Result[x.Key].Content = string.Join("  ", x.Value.ToArray());
            }
        }

        private void ReShow()
        {
            foreach (var x in LB_Result)
                x.Content = string.Empty;
            TB_R.Text = String.Empty;
            TB_INPUT.Text = String.Empty;
            LB_Count.Content = string.Empty;
            
            if (data.Count == 0)
            {
                return;
            }
            LB_Count.Content = data.Count.ToString();
            foreach (var x in data)
            {
                TB_R.Text += (x < 10 ? " " : "") + x.ToString() + (data.Count == 75 ? "" : "  ") + (data.Count % 12 == 0 ? "\r\n" : "");
            }

            foreach (var x in sortdata)
            {
                if (x.Key == 0)
                {
                    string ris = String.Empty;
                    for (int i = 0; i < x.Value.Count; i++)
                    {
                        ris += (x.Value[i] == 10 ? "" : " ") + x.Value[i].ToString() + (i == x.Value.Count - 1 ? "" : "  ");
                    }
                    LB_Result[x.Key].Content = ris;
                }
                else
                    LB_Result[x.Key].Content = string.Join("  ", x.Value.ToArray());
            }
        }
        private void ReStart()
        {
            data.Clear();
            sortdata.Clear();
            RebuildSource();
            foreach (var x in LB_Result)
                x.Content = string.Empty;
            TB_R.Text = String.Empty;
            LB_Count.Content = label.Content = string.Empty;
            TB_INPUT.Text = String.Empty;
        }
        private int GetRand()
        {
            int result;
            if (!TB_INPUT.IsEnabled)
            {
                int index = rand.Next(0, Source.Count);
                result = Source[index];
            }
            else
            {
                int _result;
                if (int.TryParse(TB_INPUT.Text, out _result))
                {
                    result = _result;
                }
                else
                {
                    result = 0;
                }
                TB_INPUT.Text = String.Empty;
            }
            return result;
        }

        List<Label> LB_Result = new List<Label>();

        List<int> data = new List<int>();
        Dictionary<int, List<int>> sortdata = new Dictionary<int, List<int>>();
        List<int> Source = new List<int>();
        int Result;
        Random rand = new Random();

        private void Window_Loaded(Object sender, RoutedEventArgs e)
        {
            LB_Result = new List<Label>() { LB_0, LB_1, LB_2, LB_3, LB_4, LB_5, LB_6, LB_7 };
            RebuildSource();
            _initialized = true;
        }
        
        private void BTN_ReStart_Click(Object sender, RoutedEventArgs e)
        {
            if (data.Count < 75)
            {
                if (MessageBox.Show("是否確定重新開始", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) == MessageBoxResult.Yes)
                    ReStart();
            }
        }
        private void RebuildSource()
        {
            Source = new List<int>();
            for (int i = 1; i < 76; i++)
                Source.Add(i);
        }

        private void BTN_Mode_Click(Object sender, RoutedEventArgs e)
        {
            TB_INPUT.IsEnabled = !TB_INPUT.IsEnabled;
        }

        private void TB_INPUT_PreviewKeyDown(Object sender, KeyEventArgs e)
        {
            if ((e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || (e.Key >= Key.D0 && e.Key <= Key.D9) ||
                    e.Key == Key.Back || e.Key == Key.Left || e.Key == Key.Right || e.Key == Key.Tab)
            {
                if (e.KeyboardDevice.Modifiers != ModifierKeys.None)
                {
                    e.Handled = true;
                }
            }
            else
            {
                e.Handled = true;
            }
            if (e.Key == Key.Enter)
            {
                BTN_OK_Click(sender, e);
            }
        }

        private bool _initialized;
        private void TB_INPUT_IsEnabledChanged(Object sender, DependencyPropertyChangedEventArgs e)
        {
            if (!_initialized)
                return;
            label.IsEnabled = !TB_INPUT.IsEnabled;
            BTN_Back.IsEnabled = TB_INPUT.IsEnabled;
            if (TB_INPUT.IsEnabled)
            {
                TB_INPUT.Visibility = BTN_Back.Visibility = BTN_Back1.Visibility = Visibility.Visible;
                label.Visibility = Visibility.Hidden;
                BTN_OK.Content = "確定";
            }
            else
            {
                TB_INPUT.Visibility = BTN_Back.Visibility = BTN_Back1.Visibility = Visibility.Hidden;
                label.Visibility = Visibility.Visible;
                BTN_OK.Content = "開始開獎";
            }
            //ReStart();
        }

        private void BTN_Back_Click(Object sender, RoutedEventArgs e)
        {
            int _result;

            if (data.Count == 0 || !int.TryParse(TB_INPUT.Text, out _result) || !data.Contains(_result))
            {
                MessageBox.Show("沒有號碼");
                return;
            }
            int index = (_result / 10) - (_result % 10 == 0 ? 1 : 0);
            data.Remove(_result);
            if (!Source.Contains(_result))
                Source.Add(_result);
            foreach (var x in sortdata)
            {
                if (x.Key == index && x.Value.Contains(_result))
                    x.Value.Remove(_result);
            }
            ReShow();
        }

        private void BTN_Back1_Click(Object sender, RoutedEventArgs e)
        {
            if (data.Count == 0)
            {
                MessageBox.Show("沒有號碼");
                return;
            }
            int _result;
            if (!string.IsNullOrWhiteSpace(TB_INPUT.Text) && int.TryParse(TB_INPUT.Text, out _result) && _result != data[data.Count - 1])
            {
                if (MessageBox.Show("是否確定刪除最後一個號碼", "警告", MessageBoxButton.YesNo, MessageBoxImage.Warning, MessageBoxResult.No) != MessageBoxResult.Yes)
                    return;
            }
            _result = data[data.Count - 1];
            int index = (_result / 10) - (_result % 10 == 0 ? 1 : 0);
            data.Remove(_result);
            if (!Source.Contains(_result))
                Source.Add(_result);
            foreach (var x in sortdata)
            {
                if (x.Key == index && x.Value.Contains(_result))
                    x.Value.Remove(_result);
            }
            ReShow();
        }
    }
}
