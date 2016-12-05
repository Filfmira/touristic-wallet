using PCLStorage;
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
        public double[] ammounts = new double[12] { 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0, 0.0 };
        public Label[] labels = new Label[12] { new Label(), new Label(), new Label(), new Label(), new Label(), new Label(), new Label(), new Label(), new Label(), new Label(), new Label(), new Label() };
        public Label[] labelsVisible = new Label[12] { new Label(), new Label(), new Label(), new Label(), new Label(), new Label(), new Label(), new Label(), new Label(), new Label(), new Label(), new Label() };
        public double[] conversionRates = new double[12] {0,0,0,0,0,0,0,0,0,0,0,0};
        public BoxView[] boxviews = new BoxView[12];
        public Color[] colors = new Color[12] { Color.Red, Color.Blue, Color.Green, Color.Orange, Color.Fuchsia, Color.Gray, Color.Yellow, Color.Maroon, Color.Purple, Color.Lime, Color.Aqua, Color.Black };


        public CallPage()
        {
            stack1 = new StackLayout()
            {
                Orientation = StackOrientation.Vertical,
            };


            var ignore = LoadData();
           

            lab1 = new Label()
            {
                HorizontalOptions = LayoutOptions.Start,
                Text = "Ammount: ",
                FontSize=20,
                TextColor=Color.Black
            };


            //loadCurrenciesConversion();
            
            foreach (Label label in labels)
            {
                label.Text = "";
            }

            var ignore2=loadCurrenciesConversion();
            
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

           

            int counter = 0;     

            var totalMoneyLab = new Label()
            {
                Text = "Calculate total money in:",
                FontSize = 15,
                TextColor = Color.Black,
                WidthRequest = 250,
                VerticalTextAlignment = TextAlignment.Center

            };

            /*
            but = new Button()
            {
                Text = "Calculate",
                HorizontalOptions = LayoutOptions.Center,
                TextColor=Color.White,
                BackgroundColor=Color.FromRgb(38, 148, 242),
                BorderColor = Color.Transparent
            };
            but.Clicked += OnButton_Clicked;*/

            var stack2 = new StackLayout()
            {
                Orientation = StackOrientation.Horizontal,
                Children = { totalMoneyLab, picker/*, but */}

            };

            var newAmmountLab = new Label()
            {
                Text = "Add/Sub ammount: ",
                FontSize=15,
                TextColor=Color.Black,
                WidthRequest = 250,
                VerticalTextAlignment= TextAlignment.Center
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
                HorizontalOptions = LayoutOptions.Start,
                BackgroundColor = Color.FromHex("#5DA865"),
                BorderColor =Color.Transparent,
                TextColor=Color.White

            };
            butAdd.Clicked += OnAddButton_Clicked;

            var butSub = new Button()
            {
                Text = "Sub",
                HorizontalOptions = LayoutOptions.End,
                BackgroundColor = Color.FromHex("#c71f16"),
                BorderColor = Color.Transparent,
                TextColor=Color.White

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
                HorizontalOptions= LayoutOptions.FillAndExpand,
                
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
            
            BoxView bv2 = new BoxView
            {
                HeightRequest = 20,
                BackgroundColor = Color.Transparent
            };


            Label title = new Label
            {
                Text = "My Wallet",
                FontSize = 25,
                TextColor = Color.Black,
                HeightRequest = 40
            };

            var contentStack = new StackLayout
            {
                Padding = new Thickness(20),
                Children = { title,stack3, bv2, lab1, bv2,stack2, bv2, stack1, bv2,layoutGraph }

            };


            var scrollview = new ScrollView
            {
                Content = contentStack,
            };

            Content = new StackLayout
            {
                Children = { scrollview }
            };


            picker.SelectedIndexChanged += OnButton_Clicked;
            //picker.SelectedIndex = 0;
            picker2.SelectedIndex = 0;
        }
        

        private void OnButton_Clicked(object sender, EventArgs e)
        {
            CalculateWallet();
        }


       
        async Task LoadData()
        {
            char[] delimiterChars = { ',' };
            String text = await ReadFromFile();

            string[] words = text.Split(delimiterChars);

            int counter = 0;
            if (words.Length != 1)
            {
                
                foreach (string s in words)
                {
                    ammounts[counter] = Convert.ToDouble(s);
                    counter++;
                }
                //lab1.Text = text;
            }

            counter = 0;
            foreach (double ammount in ammounts)
            {
                Debug.WriteLine("DENTRO DO FOR");

                if (ammount != 0)
                {
                    labelsVisible[counter] = new Label()
                    {
                        Text = currency[counter] + " in wallet: " + ammount,
                        WidthRequest = 100
                    };
                    BoxView b = new BoxView
                    {
                        WidthRequest = 10,
                        HeightRequest=10,
                        BackgroundColor = colors[counter]
                    };

                    StackLayout st = new StackLayout
                    {
                        Orientation = StackOrientation.Horizontal,
                        HorizontalOptions = LayoutOptions.Start,
                        VerticalOptions = LayoutOptions.CenterAndExpand
                    };

                    st.Children.Add(b);
                    st.Children.Add(labelsVisible[counter]);
                    stack1.Children.Add(st);
                }
                counter++;
             }




            double max = ammounts.Max();

            int counter2 = 0;
            foreach (double d in ammounts)
            {

                boxviews[counter2] = new BoxView
                {
                    WidthRequest = 20,
                    BackgroundColor = colors[counter2],
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.End
                };

                if (max == 0.0)
                    boxviews[counter2].HeightRequest = 0;
                else
                    boxviews[counter2].HeightRequest = (d * 150) / max;


                layoutGraph.Children.Add(boxviews[counter2]);
                counter2++;
            }
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
            if(picker.SelectedIndex==-1)
                picker.SelectedIndex = 0;

            totalSumCurrency = totalSumEuros * (1 / conversionRates[picker.SelectedIndex]);

            //update graph

            for (int z = 0; z < ammounts.Length; z++)
            {
                ammountsEuro[z] = ammountsEuro[z] * (1 / conversionRates[picker.SelectedIndex]);


            }
            double max = ammountsEuro.Max();

            for (int z = 0; z < ammounts.Length; z++)
            {
                if (max == 0.0)
                    boxviews[z].HeightRequest = 0;
                else
                    boxviews[z].HeightRequest = (ammountsEuro[z] * 150) / max;
            }


            lab1.Text = "Ammount:  " + Math.Round(totalSumCurrency) + " " + picker.Items[picker.SelectedIndex];
        }

        /* MUDAR FORMA COMO ESTÁ A SER REESCRITO O FORM */

        private async void OnAddButton_Clicked(object sender, EventArgs e)
        {

            if (ammounts[picker2.SelectedIndex] == 0)
            {
                ammounts[picker2.SelectedIndex] += double.Parse(value.Text);

                labelsVisible[picker2.SelectedIndex] = new Label()
                {
                    Text = currency[picker2.SelectedIndex] + " in wallet: " + double.Parse(value.Text),
                    WidthRequest = 100
                };
                
                BoxView b = new BoxView
                {
                    WidthRequest = 10,
                    HeightRequest = 10,
                    BackgroundColor = colors[picker2.SelectedIndex]
                };

                StackLayout st = new StackLayout
                {
                    Orientation = StackOrientation.Horizontal,
                    HorizontalOptions = LayoutOptions.Start,
                    VerticalOptions = LayoutOptions.CenterAndExpand
                };

                st.Children.Add(b);
                st.Children.Add(labelsVisible[picker2.SelectedIndex]);
                stack1.Children.Add(st);

            }
            else
            {
                ammounts[picker2.SelectedIndex] += double.Parse(value.Text);
                labelsVisible[picker2.SelectedIndex].Text = currency[picker2.SelectedIndex] + " in wallet: " + ammounts[picker2.SelectedIndex].ToString();

            }



            CalculateWallet();
            await WriteToFile(ammounts);
           // lab1.Text = await ReadFromFile();


        }

        private async void OnSubButton_Clicked(object sender, EventArgs e)
        {
            
            if (ammounts[picker2.SelectedIndex] == 0) { }
            else
            {
                ammounts[picker2.SelectedIndex] -= double.Parse(value.Text);
                if (ammounts[picker2.SelectedIndex] < 0) ammounts[picker2.SelectedIndex] = 0.0;
                labelsVisible[picker2.SelectedIndex].Text = currency[picker2.SelectedIndex] + " in wallet: " + ammounts[picker2.SelectedIndex].ToString();
            }

            CalculateWallet();
            await WriteToFile(ammounts);

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
                //Device.BeginInvokeOnMainThread(() => state.Item1.Text = "Status: " + response.StatusCode);
                if (response.StatusCode == HttpStatusCode.OK)
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        var content = reader.ReadToEnd();
                        Device.BeginInvokeOnMainThread(() => state.Item2.Text = content);
                    }
            }
        }

        private static async Task WriteToFile(double[] ammounts)
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;
            IFolder folder = await rootFolder.CreateFolderAsync("Wallet",
                CreationCollisionOption.OpenIfExists);
            IFile file = await folder.CreateFileAsync("wallet.txt",
                CreationCollisionOption.ReplaceExisting);
            await file.WriteAllTextAsync(string.Join(",", ammounts));
        }

        private static async Task<String> ReadFromFile()
        {
            IFolder rootFolder = FileSystem.Current.LocalStorage;

            ExistenceCheckResult existes = await rootFolder.CheckExistsAsync("Wallet");
            if (existes.ToString().Equals("NotFound"))
            {
                return "null";
            }
            else
            {
                IFolder folder = await rootFolder.GetFolderAsync("Wallet");
                IFile file = await folder.GetFileAsync("wallet.txt");
                String x = await file.ReadAllTextAsync();
                return x;
            }


        }

    }
}
