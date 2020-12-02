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
        int numberOfTickets;
        string typeOne;
        string typeTwo;
        bool isNumber;
        bool isTypeOne;
        bool isTypeTwo;
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
            float confidence = e.Result.Confidence;
            AddContent(lbl_recognition, "ROZPOZNANO: \"" + txt + "\" z pewnoscia " + confidence, false);
            if (confidence >= 0.7)
            {


                if (e.Result.Semantics["numberOfTickets"].Value.Equals(" ") || e.Result.Semantics["numberOfTickets"].Value.Equals("")) { isNumber = false; }
                else { numberOfTickets = Convert.ToInt32(e.Result.Semantics["numberOfTickets"].Value);
                    isNumber = true;
                }

                if (e.Result.Semantics["typeOne"].Value.Equals("")) { isTypeOne = false; }
                else { typeOne = e.Result.Semantics["typeOne"].Value.ToString();
                    isTypeOne = true;
                }

                if (e.Result.Semantics["typeTwo"].Value.Equals("")) { isTypeTwo = false; }
                else { typeTwo = e.Result.Semantics["typeTwo"].Value.ToString();
                    isTypeTwo = true;
                }



                if (isNumber && isTypeOne && isTypeTwo)
                {
                    ss.SpeakAsync("Mam wszystkie niezbędne dane");
                }

                else if (!isNumber && isTypeOne && isTypeTwo)
                {
                    ss.SpeakAsync("Brak liczby biletów");
                }

                else if (isNumber && !isTypeOne && isTypeTwo)
                {
                    ss.SpeakAsync("Podaj jaki bilet " + typeTwo + " chcesz kupić");
                }

                else if (isNumber && isTypeOne && !isTypeTwo)
                {
                    ss.SpeakAsync("Podaj jaki bilet " + typeOne + " chcesz kupić");
                }

                else if (!isNumber && !isTypeOne && isTypeTwo)
                {
                    
                }
                if (isNumber && !isTypeOne && !isTypeTwo)
                {
                    
                }
                if (!isNumber && isTypeOne && !isTypeTwo)
                {
                    
                }
                if (!isNumber && !isTypeOne && !isTypeTwo)
                {

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
