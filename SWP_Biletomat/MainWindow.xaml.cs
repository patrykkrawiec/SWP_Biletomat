using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;

namespace SWP_Biletomat
{
    //folder z gramatyką trzeba wrzucić do \bin\x64\Debug w folderze projektu, 
    //lub zaznaczyć we właściwościach "kopiuj zawsze" dla "Kopiuj dla katalogu wyjściowego" i "zawartość" dla "Akcja kompilacji"

    //TODO: Uniemożliwić zmniejszanie okna do bardzo małego 
    public partial class MainWindow : Window
    {
        static SpeechRecognitionEngine sre;
        static SpeechSynthesizer ss;
        private readonly BackgroundWorker worker = new BackgroundWorker();
        private int numberOfTickets=0;
        private string timeType="";
        private string type="";
        private bool isNumber=false;
        private bool isTimeType = false;
        private bool isType = false;
        public MainWindow()
        {
            InitializeComponent();
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerAsync();

        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e)
        {
            ss = new SpeechSynthesizer();
            ss.SetOutputToDefaultAudioDevice();
            ss.Speak("Witaj, jaki bilet potrzebujesz?");
            CultureInfo ci = new CultureInfo("pl-PL");
            sre = new SpeechRecognitionEngine(ci);
            sre.SetInputToDefaultAudioDevice();
            sre.SpeechRecognized += Sre_SpeechRecognized;
            Grammar grammar = new Grammar(".\\Grammars\\Grammar.xml", "rootRule");
            grammar.Enabled = true;
            sre.LoadGrammar(grammar);
            sre.RecognizeAsync(RecognizeMode.Multiple);
            AddContent(lbl_help, "UŻYJ SFORMUŁOWANIA: ", false);
            AddContent(lbl_exemple, "PRZYKŁAD: ", false);
        }

        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            string txt = e.Result.Text;
            Console.WriteLine(txt);
            float confidence = e.Result.Confidence;
            AddContent(lbl_recognition, "ROZPOZNANO: \"" + txt + "\" z pewnoscia " + confidence, false);
            if (confidence >= 0.8)
            {


                if (e.Result.Semantics["numberOfTickets"].Value.Equals(" ") && !isNumber ||e.Result.Semantics["numberOfTickets"].Value.Equals("") && !isNumber) { isNumber = false; }
                else if(!isNumber) { numberOfTickets = Convert.ToInt32(e.Result.Semantics["numberOfTickets"].Value);
                    isNumber = true;
                }

                if (e.Result.Semantics["timeType"].Value.Equals("") && !isTimeType) { isTimeType = false; }
                else if(!isTimeType) { timeType = e.Result.Semantics["timeType"].Value.ToString();
                    isTimeType = true;
                }

                if (e.Result.Semantics["type"].Value.Equals("") && !isType) { isType = false; }
                else if (!isType)
                { type = e.Result.Semantics["type"].Value.ToString();
                    isType = true;
                }



                if (isNumber && isTimeType && isType)
                {
                    ss.SpeakAsync("Mam wszystkie niezbędne dane");
                    ss.SpeakAsync("Wybrano " + numberOfTickets + " bilet " + timeType + ", " + type);
                }

                else if (!isNumber && isTimeType && isType)
                {
                    ss.SpeakAsync("Wybrano bilet "+ timeType +", "+ type + ", podaj liczbę biletów");
                }

                else if (isNumber && !isTimeType && isType)
                {
                    ss.SpeakAsync("Wybrano " + numberOfTickets + " bilet " + type + ", podaj czas trwania biletu");
                }

                else if (isNumber && isTimeType && !isType)
                {
                    ss.SpeakAsync("Wybrano " + numberOfTickets + " bilet " + timeType + ", chcesz kupić bilet normalny czy ulgowy?");
                }

                else if (!isNumber && !isTimeType && isType)
                {
                    ss.SpeakAsync("Brak liczby i okresu");
                }
                if (isNumber && !isTimeType && !isType)
                {
                    ss.SpeakAsync("Brak okresu i typu");
                }
                if (!isNumber && isTimeType && !isType)
                {
                    ss.SpeakAsync("Brak liczby i typu");
                }
                if (!isNumber && !isTimeType && !isType)
                {
                    ss.SpeakAsync("Brak każdej z trzech");
                }

            }
            else
            {
                ss.SpeakAsync("Proszę powtórzyć");
            }
        }

        private void AddContent(Label label, String content, bool setVisibility)
        {
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                label.Content = content;
            }));
        }


    }
}
