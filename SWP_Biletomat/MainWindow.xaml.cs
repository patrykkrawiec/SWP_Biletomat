using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Globalization;
using Microsoft.Speech.Recognition;
using Microsoft.Speech.Synthesis;
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
        private string instruction = "UŻYJ SFORMUŁOWANIA:    \"Poproszę bilet\" + (Odpowiedź systemu) + liczba biletów + rodzaj bieletu ";
        private string example = "PRZYKŁAD:    \"Poproszę bilet, ... jeden ulgowy dwudziestominutowy\" ";
        private bool initialCondition = true;
        private string recognized; 
        private String ticketString;
        private float ticketPrice=0;
        private Numeral numeral = new Numeral();
        private void Button_Click(object sender, RoutedEventArgs e){
            paymentStatus = 1;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e){
            paymentStatus = 2;
        }

        public MainWindow(){
            InitializeComponent();
            Instrucion_lbl.Content = instruction;
            Exemple_lbl.Content = example;
            worker.DoWork += Worker_DoWork;
            worker.RunWorkerAsync();
        }

        private void Worker_DoWork(object sender, DoWorkEventArgs e){
            ss = new SpeechSynthesizer();
            ss.SetOutputToDefaultAudioDevice();
            ss.Speak("Witam");
            CultureInfo ci = new CultureInfo("pl-PL");
            sre = new SpeechRecognitionEngine(ci);
            sre.SetInputToDefaultAudioDevice();
            sre.SpeechRecognized += Sre_SpeechRecognized;

            grammarStart = new Grammar(".\\Grammar\\StartGrammar.xml", "rootRule");
            grammarTickets = new Grammar(".\\Grammar\\Tickets.xml", "rootRule");
            grammarFollowingOperation = new Grammar(".\\Grammar\\FollowingOperation.xml", "rootRule");
            grammarPayment = new Grammar(".\\Grammar\\Payment.xml", "rootRule");

            sre.LoadGrammar(grammarStart);
            sre.LoadGrammar(grammarTickets);
            sre.LoadGrammar(grammarFollowingOperation);
            sre.LoadGrammar(grammarPayment); 
            grammarStart.Enabled = true;

            sre.RecognizeAsync(RecognizeMode.Multiple);

        }

        private bool askForAdditions = false;

        private void Sre_SpeechRecognized(object sender, SpeechRecognizedEventArgs e)
        {
            recognized = e.Result.Text;

            Console.WriteLine(recognized);
            float confidence = e.Result.Confidence;
            if (confidence >= 0.7)
            {
                if ( initialCondition && !e.Result.Semantics["action"].Value.Equals("") && !e.Result.Semantics["ticket"].Value.Equals(""))
                {
                    grammarTickets.Enabled = true;
                    grammarStart.Enabled = false;
                    order = new Order();
                    order.Tickets = new List<Ticket>();

                    this.Dispatcher.BeginInvoke(new Action(() =>{
                        Recognized_lbl.Content = "ROZPOZNANO: " + recognized;
                        History_tb.Text += recognized;
                    }));

                    showTicketMachineStatment("Proszę podać liczbe i rodzaj biletu");
                    initialCondition = false;
                }
                else
                {

                    update();

                    if (askForAdditions){
                        updateStatus(e);
                        askForAdditions = false;
                    }
                    else if (!order.isDone){
                        getNewTickets(e);
                    }
                    else if (!order.isPaid){
                        payForTickets(e);
                    }

                    update();

                }
            }
            else{
                showTicketMachineStatment("Proszę powtórzyć");
            }
        }
        private void getNewTickets(SpeechRecognizedEventArgs e)
        {
            if (!e.Result.Semantics["ticket_type"].Value.Equals("") && !e.Result.Semantics["ticket_time"].Value.Equals("")) {
                askForAdditions = true;

                Ticket ticketFinder = new Ticket();
                Ticket.TicketType type = ticketFinder.setTicketType(e.Result.Semantics["ticket_type"].Value.ToString() + e.Result.Semantics["ticket_time"].Value.ToString());

                if (order.Tickets.Exists(x => x.ticketType == type)){
                    int ticketsCount = Convert.ToInt32(e.Result.Semantics["number"].Value);
                    order.Tickets.First(x => x.ticketType == type).count += ticketsCount;

                    ticketString = "\n" + ticketsCount + " x " + type;
                    ticketPrice += ticketsCount * order.Tickets.First(x => x.ticketType == type).ticketPrice;
                    Console.WriteLine(ticketPrice.ToString());
                    order.Tickets.First(x => x.ticketType == type).setTicketTypeName();
                }
                else{
                    Ticket ticket = new Ticket();
                    ticket.count = Convert.ToInt32(e.Result.Semantics["number"].Value);
                    ticket.ticketType = type;
                    order.Tickets.Add(ticket);

                    ticketString = "\n"+ticket.count.ToString() + " x " + ticket.ticketType.ToString();
                    ticket.setTicketPrice();
                    ticketPrice += ticket.count * ticket.ticketPrice;
                    ticket.setTicketTypeName();
                }

                grammarTickets.Enabled = false;
                grammarFollowingOperation.Enabled = true;
                showTicketMachineStatment("Czy kontynuować zamówienie?");
            }
            else{
                showTicketMachineStatment("Błąd w poleceniu, proszę podać liczbę biletów i rodzaj biletu");
            }

        }




        private void payForTickets(SpeechRecognizedEventArgs e)
        {
            if (e.Result.Semantics["payment_type"].Value.ToString() == "karta")
            {
                showTicketMachineStatment("Przyłóż kartę do czytnika");
                while (paymentStatus != 1){}
                order.isPaid = true;
                grammarPayment.Enabled = false;
                showTicketMachineStatment("Płatność zakończona, dziękuję. Do widzenia.");
            }
            if (e.Result.Semantics["payment_type"].Value.ToString() == "gotówka")
            {
                showTicketMachineStatment("Wrzuć monety");
                while (paymentStatus != 2){}
                order.isPaid = true;
                grammarPayment.Enabled = false;
                showTicketMachineStatment("Płatność zakończona, dziękuję. Do widzenia.");
            }
        }

        private void updateStatus(SpeechRecognizedEventArgs e)
        {
            if (e.Result.Semantics["following_operation"].Value.ToString() == "tak")
            {
                grammarFollowingOperation.Enabled = false;
                grammarTickets.Enabled = true;
                showTicketMachineStatment("Prosze wskazać bilet");

            }
            if (e.Result.Semantics["following_operation"].Value.ToString() == "nie")
            {
                grammarFollowingOperation.Enabled = false;
                showTicketMachineStatment("Wybrane przez Ciebie bilety to ");
                order.isDone = true;
                foreach (Ticket bilet in order.Tickets)
                {
                    showTicketMachineStatment(numeral.numList[bilet.count] + " " + bilet.ticketTypeName);
                }
                showTicketMachineStatment("Płatność kartą czy gotówką ?");
                grammarFollowingOperation.Enabled = false;
                grammarPayment.Enabled = true;
            }
        }


        private void showTicketMachineStatment(String s)
        {
            ss.Speak(s);

            this.Dispatcher.BeginInvoke(new Action(() => {
                String temp = History_tb.Text;
                History_tb.Text = "-"+s;
                History_tb.Text += "\n" + temp;

            }));
        }

        private void update()
        {
            this.Dispatcher.BeginInvoke(new Action(() => {
                if (!recognized.Equals(""))
                {
                    String temp = History_tb.Text;
                    History_tb.Text = recognized;
                    History_tb.Text += "\n" + temp;
                    Recognized_lbl.Content = "ROZPOZNANO: " + recognized;
                }
                Ticket_viewer.Content += ticketString;
                Price_lbl2.Content = ticketPrice.ToString();
                ticketString = "";
                recognized = "";

            }));
        }
    }
}
