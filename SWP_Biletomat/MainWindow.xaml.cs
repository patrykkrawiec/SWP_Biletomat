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
        private Order order;
        private int paymentStatus = 0;
        private readonly BackgroundWorker worker = new BackgroundWorker();
        Grammar grammarStart, grammarTickets, grammarFollowingOperation, grammarPayment;
        private int fail_count = 0;
        private string avaible_oper = "";
        private string instruction = "Powiedz \" ile jest \" a następnie działanie w formacie \"liczba\" operacja \"liczba\" ";

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            paymentStatus = 1;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            paymentStatus = 2;
        }

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
            ss.Speak("Witam");
            CultureInfo ci = new CultureInfo("pl-PL");
            sre = new SpeechRecognitionEngine(ci);
            sre.SetInputToDefaultAudioDevice();
            sre.SpeechRecognized += Sre_SpeechRecognized;

            GrammarBuilder buildGrammarSystem = new GrammarBuilder();
            buildGrammarSystem.Append("poproszę bilet");
            grammarStart = new Grammar(buildGrammarSystem);
            sre.LoadGrammar(grammarStart);
            grammarStart.Enabled = true;

            grammarTickets = new Grammar(".\\Grammar\\Tickets.xml", "rootRule");
            sre.LoadGrammar(grammarTickets);

            grammarFollowingOperation = new Grammar(".\\Grammar\\FollowingOperation.xml", "rootRule");
            sre.LoadGrammar(grammarFollowingOperation);

            grammarPayment = new Grammar(".\\Grammar\\Payment.xml", "rootRule");
            sre.LoadGrammar(grammarPayment);

            sre.RecognizeAsync(RecognizeMode.Multiple);
            this.Dispatcher.BeginInvoke(new Action(() =>
            {
                Failed_rec_lbl.Content = "Niepoprawnie rozpoznanych instrukcji";
                avaible_operation.Content = avaible_oper;
                avaible_operation.Visibility = Visibility.Visible;
                Instrucion_lbl.Visibility = Visibility.Visible;
                Speech_lbl.Visibility = Visibility.Visible;
                Recognized_lbl.Visibility = Visibility.Visible;
            }));

        }
        private bool askForAdditions = false;
        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {

            string txt = e.Result.Text;

            Console.WriteLine(txt);
            float confidence = e.Result.Confidence;
            if (confidence >= 0.7)
            {
                if (txt.IndexOf("poproszę bilet") >= 0)
                {
                    grammarTickets.Enabled = true;
                    grammarStart.Enabled = false;
                    order = new Order();
                    order.Tickets = new List<Ticket>();
                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {

                        Recognized_lbl.Content = "Rozpoznano :" + txt;
                    }));

                }
                else
                {
                    if (askForAdditions)
                    {
                        updateStatus(e);
                        askForAdditions = false;

                    }
                    else if (!order.isDone)
                    {

                        getNewTickets(e);
                        askForAdditions = true;

                    }
                    else if (!order.isPaid)
                    {
                        payForTickets(e);
                    }



                    this.Dispatcher.BeginInvoke(new Action(() =>
                    {
                        Recognized_lbl.Content = "Rozpoznano :" + txt;
                    }));
                }


            }
            else
            {
                ss.Speak("Proszę powtórzyć");

            }
        }
        private void getNewTickets(SpeechRecognizedEventArgs e)
        {
            if (e.Result.Semantics["ticket_type"].Value.ToString() == "normalny")
            {
                if (order.Tickets.Exists(x => x.ticketType == Ticket.TicketType.Normalny))
                {
                    int ticketsCount = Convert.ToInt32(e.Result.Semantics["number"].Value);
                    order.Tickets.First(x => x.ticketType == Ticket.TicketType.Normalny).count += ticketsCount;

                }
                else
                {
                    Ticket ticket = new Ticket();
                    ticket.count = Convert.ToInt32(e.Result.Semantics["number"].Value);
                    ticket.ticketType = Ticket.TicketType.Normalny;
                    order.Tickets.Add(ticket);
                }

                grammarTickets.Enabled = false;
                grammarFollowingOperation.Enabled = true;
                ss.Speak("czy kontynuować zamówienie");
            }
            if (e.Result.Semantics["ticket_type"].Value.ToString() == "ulgowy")
            {
                if (order.Tickets.Exists(x => x.ticketType == Ticket.TicketType.Ulgowy))
                {
                    int ticketsCount = Convert.ToInt32(e.Result.Semantics["number"].Value);
                    order.Tickets.First(x => x.ticketType == Ticket.TicketType.Ulgowy).count += ticketsCount;

                }
                else
                {
                    Ticket ticket = new Ticket();
                    ticket.count = Convert.ToInt32(e.Result.Semantics["number"].Value);
                    ticket.ticketType = Ticket.TicketType.Ulgowy;
                    order.Tickets.Add(ticket);
                }
                grammarTickets.Enabled = false;
                grammarFollowingOperation.Enabled = true;
                ss.Speak("czy kontynuować zamówienie");
            }

        }
        private void payForTickets(SpeechRecognizedEventArgs e)
        {
            if (e.Result.Semantics["payment_type"].Value.ToString() == "karta")
            {
                ss.Speak("Przyłóż kartę do czytnika");
                while (paymentStatus != 1)
                {

                }
                order.isPaid = true;
                grammarPayment.Enabled = false;
                ss.Speak("Płatność zakończona, dziękuję");
            }
            if (e.Result.Semantics["payment_type"].Value.ToString() == "gotówka")
            {
                ss.Speak("Wrzuć monety");
                while (paymentStatus != 2)
                {

                }
                order.isPaid = true;
                grammarPayment.Enabled = false;
                ss.Speak("Płatność zakończona, dziękuję");
            }
        }
        private void updateStatus(SpeechRecognizedEventArgs e)
        {
            if (e.Result.Semantics["following_operation"].Value.ToString() == "tak")
            {
                grammarFollowingOperation.Enabled = false;
                grammarTickets.Enabled = true;
                ss.Speak("prosze wskazać bilet");

            }
            if (e.Result.Semantics["following_operation"].Value.ToString() == "nie")
            {
                grammarFollowingOperation.Enabled = false;
                ss.Speak("zamówienie składa się z.");
                order.isDone = true;
                foreach (Ticket bilet in order.Tickets)
                {
                    ss.Speak(bilet.count + ". " + bilet.ticketType.ToString());
                }
                ss.Speak("Płatność kartą czy gotówką ?");
                grammarFollowingOperation.Enabled = false;
                grammarPayment.Enabled = true;
            }
        }
    }
}
