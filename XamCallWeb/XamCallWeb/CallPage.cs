using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace XamCallWeb
{
    public class CallPage : ContentPage
    {
        Label lab1, lab2;
        Entry value;
        Picker picker, picker2;
        StackLayout stack1;
        StackLayout layoutGraph;
        Button but;

        public string[] currency = new string[12] { "EUR", "USD", "GBP", "CHF", "CNY", "CAD", "BRL", "AUD", "INR", "JPY", "RUB", "NZD" };

        public double[] ammounts = new double[12] { 1500, 300, 300, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };

        public Label[] labels = new Label[12] { new Label(), new Label(), new Label(), new Label(), new Label(), new Label(), new Label(), new Label(), new Label(), new Label(), new Label(), new Label() };

        public Label[] labelsVisible = new Label[12] { new Label(), new Label(), new Label(), new Label(), new Label(), new Label(), new Label(), new Label(), new Label(), new Label(), new Label(), new Label() };


        public double[] conversionRates = new double[12] {0,0,0,0,0,0,0,0,0,0,0,0};

        public BoxView[] boxviews = new BoxView[12];


        public CallPage()
        {

            lab1 = new Label()
            {
                HorizontalOptions = LayoutOptions.Center,
                Text = "Ammount: "
            };


            //loadCurrenciesConversion();
            
            foreach (Label label in labels)
            {
                label.Text = "";
            }
            loadCurrenciesConversion();


            picker = new Picker()
            {
                Title = "Currency",
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };

            picker2 = new Picker()
            {
                Title = "Currency",
                VerticalOptions = LayoutOptions.CenterAndExpand,
            };

            foreach (string colorName in currency)
            {
                picker.Items.Add(colorName);
                picker2.Items.Add(colorName);

            }

            picker.SelectedIndex = 0;
            picker2.SelectedIndex = 0;


            stack1 = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
            };

        

            int counter = 0;

            foreach (double ammount in ammounts)
            {
                if (ammount != 0)
                {
                    labelsVisible[counter] = new Label()
                    {
                        Text = currency[counter] + " in wallet: " + ammount,
                        WidthRequest = 100
                    };

                    stack1.Children.Add(labelsVisible[counter]);
                }
                counter++;


            }

            var totalMoneyLab = new Label()
            {
                Text = "Calculate total money in:",
                WidthRequest = 250
            };


            but = new Button()
            {
                Text = "Calculate",
                HorizontalOptions = LayoutOptions.Center
            };
            but.Clicked += OnButton_Clicked;

            var stack2 = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Children = { totalMoneyLab, picker }

            };

            var newAmmountLab = new Label()
            {
                Text = "Add/Sub ammount: ",
                WidthRequest = 250
            };


            value = new Entry()
            {
                HorizontalOptions = LayoutOptions.StartAndExpand,
                Placeholder = "value",
                Keyboard = Keyboard.Numeric
            };

            var butAdd = new Button()
            {
                Text = "Add",
                HorizontalOptions = LayoutOptions.Center
            };
            butAdd.Clicked += OnAddButton_Clicked;

            var butSub = new Button()
            {
                Text = "Sub",
                HorizontalOptions = LayoutOptions.Center
            };
            butSub.Clicked += OnSubButton_Clicked;

            var stack31 = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Children = { newAmmountLab, value , picker2, butAdd, butSub }

            };

            var stack32 = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Children = { butAdd, butSub }

            };

            var stack3 = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
                Children = { stack31, stack32}

            };


            StackLayout layout = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand

            };

            layoutGraph = new StackLayout
            {
                Orientation = StackOrientation.Horizontal,
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.CenterAndExpand

            };

            double max = ammounts.Max();

            int counter2 = 0;
            foreach (double d in ammounts)
            {

                boxviews[counter2] = new BoxView
                {
                    WidthRequest = 20,
                    HeightRequest = (d * 150) / max,
                    BackgroundColor = Color.Red,
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.End
                };
                layoutGraph.Children.Add(boxviews[counter2]);
                counter2++;
            }
            var contentStack = new StackLayout
            {
                Padding = new Thickness(20),
                Children = { stack1, stack2, but, lab1, stack3, layoutGraph }

            };

            var scrollview = new ScrollView
            {
                Content = contentStack,
            };

            Content = new StackLayout
            {
                Children = { scrollview }
            };

        }

        private void OnButton_Clicked(object sender, EventArgs e)
        {
            CalculateWallet();
        }


        private void CalculateWallet()
        {
            /* OBTER SÓ O CONVERSION RATE */
            int i = 0;
            double totalSumEuros = ammounts[0];
            double totalSumCurrency = 0;
            double[] ammountsEuro = new double[12];
            ammountsEuro[0] = ammounts[0];

            foreach (Label x in labels)
            {
                List<String> names = x.Text.Split(',').ToList<String>();
                conversionRates[i] = double.Parse(names[1]);
                i++;
            }

            /* FAZER CONTAS */
            for (int z = 1; z < ammounts.Length; z++)
            {
                totalSumEuros += ammounts[z] * conversionRates[z];
                ammountsEuro[z] = ammounts[z] * conversionRates[z]; //keep values on an array
            }

            totalSumCurrency = totalSumEuros * (1 / conversionRates[picker.SelectedIndex]);

            //update graph

            for (int z = 0; z < ammounts.Length; z++)
            {
                ammountsEuro[z] = ammountsEuro[z] * (1 / conversionRates[picker.SelectedIndex]);


            }
            double max = ammountsEuro.Max();

            for (int z = 0; z < ammounts.Length; z++)
            {
                boxviews[z].HeightRequest = (ammountsEuro[z] * 150) / max;
            }


            lab1.Text = "Ammount: " + totalSumCurrency + " in " + picker.Items[picker.SelectedIndex];
        }

        /* MUDAR FORMA COMO ESTÁ A SER REESCRITO O FORM */

        private void OnAddButton_Clicked(object sender, EventArgs e)
        {

            if (ammounts[picker2.SelectedIndex] == 0)
            {
                ammounts[picker2.SelectedIndex] += double.Parse(value.Text);

                labelsVisible[picker2.SelectedIndex] = new Label()
                {
                    Text = currency[picker2.SelectedIndex] + " in wallet: " + double.Parse(value.Text),
                    WidthRequest = 100
                };

                stack1.Children.Add(labelsVisible[picker2.SelectedIndex]);

            } else {
                ammounts[picker2.SelectedIndex] += double.Parse(value.Text);
                labelsVisible[picker2.SelectedIndex].Text = currency[picker2.SelectedIndex] + " in wallet: " + ammounts[picker2.SelectedIndex].ToString();

            }



            CalculateWallet();


        }

        private void OnSubButton_Clicked(object sender, EventArgs e)
        {
            if (ammounts[picker2.SelectedIndex] == 0) { }
            else
            {
                ammounts[picker2.SelectedIndex] -= double.Parse(value.Text);
                labelsVisible[picker2.SelectedIndex].Text = currency[picker2.SelectedIndex] + " in wallet: " + ammounts[picker2.SelectedIndex].ToString();
            }

            CalculateWallet();
        }



        async public Task loadCurrenciesConversion()
        {
            /* FAZ PEDIDOS DE CONVERSION RATE DE TODAS AS CURRENCIES */
            int i = 0;
            double sumInEuro = 0.0;

            foreach (double ammount in ammounts)
            {
                var uri = string.Format("http://download.finance.yahoo.com/d/quotes?f=sl1d1t1&s={0}{1}=X", currency[i], "EUR");

                Debug.WriteLine(uri);

                var cb = new AsyncCallback(CallHandler);
                CallWebAsync(uri, lab1, labels[i], cb);


                i++;
            }


        }

        private void CallWebAsync(string uri, Label status, Label response, AsyncCallback cb)
        {
            var request = HttpWebRequest.Create(uri);
            request.Method = "GET";
            var state = new Tuple<Label, Label, WebRequest>(status, response, request);

            request.BeginGetResponse(cb, state);
        }

        private void CallHandler(IAsyncResult ar)
        {
            var state = (Tuple<Label, Label, WebRequest>)ar.AsyncState;
            var request = state.Item3;

            using (HttpWebResponse response = request.EndGetResponse(ar) as HttpWebResponse)
            {
                Device.BeginInvokeOnMainThread(() => state.Item1.Text = "Status: " + response.StatusCode);
                if (response.StatusCode == HttpStatusCode.OK)
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        Device.BeginInvokeOnMainThread(() => state.Item2.Text = content);
                    }
            }
        }

    }
}
