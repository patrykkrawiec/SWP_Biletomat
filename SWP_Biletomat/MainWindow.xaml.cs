﻿using System;
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
using System.Globalization;
using Microsoft.Speech;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
using System.Windows.Threading;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SWP_Biletomat
{
    /// <summary>
    /// Logika interakcji dla klasy MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        static SpeechRecognitionEngine sre;
        static SpeechSynthesizer ss;
        private readonly BackgroundWorker worker = new BackgroundWorker();
        private int fail_count = 0;
        private string avaible_oper = "Operacje rozpoznawane \n[+] -> plus, dodaj \n[-] ->minus,odejmij\n[*] -> razy,pomnóż\n[:] -> przez,podziel";
        private string instruction = "Powiedz \" ile jest \" a następnie działanie w formacie \"liczba\" operacja \"liczba\" ";
        public MainWindow()
        {
            InitializeComponent();
            Instrucion_lbl.Content = instruction;
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerAsync();

        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            ss = new SpeechSynthesizer();
            ss.SetOutputToDefaultAudioDevice();
            ss.Speak("Witam w kalkulatorze");
            ss.Speak("Powiedz. ile jest. a następnie działanie w formacie liczba operacja liczba ");
            ss.Speak("Obsługiwane operacje wyświetlają się po prawej stronie okna");
            CultureInfo ci = new CultureInfo("pl-PL");
            sre = new SpeechRecognitionEngine(ci);
            sre.SetInputToDefaultAudioDevice();
            sre.SpeechRecognized += Sre_SpeechRecognized;
            Grammar grammar = new Grammar(".\\Grammar\\SimpleGrammar.xml", "rootRule");
            grammar.Enabled = true;
            sre.LoadGrammar(grammar);
            sre.RecognizeAsync(RecognizeMode.Multiple);
            this.Dispatcher.BeginInvoke(new Action(() => {
                Failed_rec_lbl.Content = "Niepoprawnie rozpoznanych instrukcji";
                avaible_operation.Content = avaible_oper;
                avaible_operation.Visibility = Visibility.Visible;
                Instrucion_lbl.Visibility = Visibility.Visible;
                Speech_lbl.Visibility = Visibility.Visible;
                Recognized_lbl.Visibility = Visibility.Visible;
            }));

        }

        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {

            string txt = e.Result.Text;

            Console.WriteLine(txt);
            float confidence = e.Result.Confidence;
            if (confidence >= 0.5)
            {
                this.Dispatcher.BeginInvoke(new Action(() => {
                    Recognized_lbl.Content = "Rozpoznano :" + txt;
                }));
                int first = Convert.ToInt32(e.Result.Semantics["first"].Value);
                int second = Convert.ToInt32(e.Result.Semantics["second"].Value);
                string operation = e.Result.Semantics["operation"].Value.ToString();

                if (operation == "suma")
                {
                    int sum = first + second;
                    if (sum == 0)
                    {
                        ss.Speak("Wynik dodawania wynosi zero");

                    }
                    else
                    {
                        ss.Speak("Wynik dodawania wynosi" + sum.ToString());

                    }
                    this.Dispatcher.BeginInvoke(new Action(() => {
                        Speech_lbl.Content = "Wynik dodawania wynosi " + sum.ToString();
                        History_tb.Text += first + " " + "+ " + second + " = " + sum + "\n";
                    }));


                }
                else if (operation == "roznica")
                {
                    int sum = first - second;
                    if (sum == 0)
                    {
                        ss.Speak("Wynik odejmowania wynosi zero");

                    }
                    else
                    {
                        if (sum < 0)
                        {
                            ss.Speak("Wynik odejmowania wynosi minus" + sum.ToString());

                        }
                        else
                        {
                            ss.Speak("Wynik odejmowania wynosi" + sum.ToString());

                        }

                    }
                    this.Dispatcher.BeginInvoke(new Action(() => {
                        Speech_lbl.Content = "Wynik odejmowania wynosi " + sum.ToString();
                        History_tb.Text += first + " " + "- " + second + " = " + sum + "\n";

                    }));

                }
                else if (operation == "iloczyn")
                {
                    int sum = first * second;
                    if (sum == 0)
                    {
                        ss.Speak("Wynik mnożenia wynosi zero");

                    }
                    else
                    {
                        ss.Speak("Wynik mnożenia wynosi" + sum.ToString());

                    }
                    this.Dispatcher.BeginInvoke(new Action(() => {
                        Speech_lbl.Content = "Wynik mnożenia wynosi " + sum.ToString();
                        History_tb.Text += first + " " + "* " + second + " = " + sum + "\n";

                    }));
                }
                else if (operation == "iloraz")
                {
                    if (second != 0)
                    {
                        int sum = first / second;
                        if (sum == 0)
                        {
                            if (first % second == 0)
                            {
                                ss.Speak("wynik dzielenia wynosi zero" + " reszty zero");

                            }
                            else
                            {
                                ss.Speak("wynik dzielenia wynosi zero" + " reszty " + first % second);

                            }

                        }
                        else
                        {
                            if (first % second == 0)
                            {
                                ss.Speak("wynik dzielenia wynosi " + sum.ToString() + " reszty zero");

                            }
                            else
                            {
                                ss.Speak("wynik dzielenia wynosi " + sum.ToString() + " reszty " + first % second);

                            }

                        }
                        this.Dispatcher.BeginInvoke(new Action(() => {
                            Speech_lbl.Content = "Wynik dzielenia wynosi " + sum.ToString() + " reszty " + first % second;
                            History_tb.Text += first + " " + ": " + second + " = " + sum + " reszty " + first % second + "\n";

                        }));
                    }
                    else
                    {
                        ss.Speak("nie dziel przez zero");
                        this.Dispatcher.BeginInvoke(new Action(() => {
                            Speech_lbl.Content = "Nie dziel przez zero ";
                        }));

                    }
                }
            }
            else
            {
                ss.Speak("Proszę powtórzyć");
                this.Dispatcher.BeginInvoke(new Action(() => {
                    Failed_rec_lbl.Visibility = Visibility.Visible;
                    fail_count++;
                    failed_count.Content = fail_count;
                    Recognized_lbl.Content = "Nie rozpoznano, proszę powtórzyć ";
                }));
            }
        }
    }
}
