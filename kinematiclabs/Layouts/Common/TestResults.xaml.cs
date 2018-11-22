using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using OxyPlot;
using OxyPlot.Axes;
using OxyPlot.Series;
using OxyPlot.Xamarin.Forms;
using Plugin.GoogleAnalytics;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace kinematiclabs
{
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TestResults : ContentPage
    {
        public TestClass mytest;
        public TestClass mytest2;
        public TestClass testObj;
        public string angulototal;
        public decimal potencia_media;
        public decimal velocidad_media;
        public int i;

        public LineSeries serievelocidad;
        public LineSeries serieaceleracion;

        public TestResults()
        {

        }

        public TestResults(TestClass test)
        {
            mytest = test;
            mytest2 = test;
            potencia_media = 0;
            velocidad_media = 0;

            serievelocidad = new LineSeries
            {
                YAxisKey = "2",
                Title = "Velocidad"
            };
            serieaceleracion = new LineSeries
            {
                YAxisKey = "3",
                Title = "Aceleración"
            };

            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            GoogleAnalytics.Current.Tracker.SendView("TestResults");

            angulototal = "";
            //Grid datagrid = SetDatatable();
            Grid resultgrid = SetResult();

            //Grid.SetRow(datagrid, 0);
            //mycontent.Children.Add(datagrid);
            Grid.SetRow(resultgrid, 3);
            mycontent.Children.Add(resultgrid);

            if (String.IsNullOrEmpty(mytest.Id))
            {
                Task.Run(SaveTest).Wait();
                if (testObj.Id == null)
                    Task.Run(SaveTest).Wait();
                TitleLabel.Text = "Test " + testObj.Id;
                infofechaText.Text = testObj.Testdate.ToShortDateString();
                infohoraText.Text = testObj.Testdate.ToShortTimeString();
            }
            else
            {
                TitleLabel.Text = "Test " + mytest.Id;
                infofechaText.Text = mytest.Testdate.ToShortDateString();
                infohoraText.Text = mytest.Testdate.ToShortTimeString();
            }

            infosujetoText.Text = mytest.Fellow.FullName;
            infotipoText.Text = mytest.TypeTest.Name;
            infolateralidadText.Text = mytest.Leftright;
            inforomText.Text = angulototal;
            infopotenciaText.Text = decimal.Round(potencia_media, 2, MidpointRounding.AwayFromZero) + " W";
            infovelocidadText.Text = decimal.Round(velocidad_media, 2, MidpointRounding.AwayFromZero) + " rad/s";



            var backImage = new Image
            {
                Source = "back_cuadro.png",
                Aspect = Aspect.Fill
            };

            Grid.SetRow(backImage, 5);
            mycontent.Children.Add(backImage);

            var myplot = new PlotView
            {
                Model = new PlotModel
                {
                    Title = "",
                    TextColor = OxyColor.FromRgb(255, 255, 255),
                    PlotAreaBorderColor = OxyColor.FromRgb(255, 255, 255), 
                    LegendPlacement = LegendPlacement.Outside,
                    LegendPosition = LegendPosition.TopCenter,
                    Padding = new OxyThickness(10),
                    Axes =
                    {
                        new LinearAxis {
                            Position = AxisPosition.Bottom,
                            Minimum = 0,
                            AxislineColor = OxyColor.FromRgb(255,255,255),
                            Key = "1",
                            IsPanEnabled = false,
                            IsZoomEnabled = false,
                            Title = "Tiempo (s)",
                            TicklineColor = OxyColor.FromRgb(255,255,255)
                        },
                        new LinearAxis {
                            Position = AxisPosition.Left,
                            AxislineColor = OxyColor.FromRgb(255,255,255),
                            Key = "2",
                            IsPanEnabled = false,
                            IsZoomEnabled = false,
                            Title = "rad/s",
                            TicklineColor = OxyColor.FromRgb(255,255,255)
                        },
                        new LinearAxis {
                            Position = AxisPosition.Right,
                            AxislineColor = OxyColor.FromRgb(255,255,255),
                            Key= "3",
                            IsPanEnabled = false,
                            IsZoomEnabled = false,
                            Title = "rad/s\xB2",
                            TicklineColor = OxyColor.FromRgb(255,255,255),
                        }
                    },
                    Series =
                    {
                        serievelocidad, serieaceleracion

                    }
                },
                VerticalOptions = LayoutOptions.Fill,
                HorizontalOptions = LayoutOptions.Fill
            };

            Grid.SetRow(myplot, 5);
            mycontent.Children.Add(myplot);

        }

        public Grid SetDatatable()
        {
            i = 0;

            var main1 = new Grid()
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.White,
                RowSpacing = 0,
                Padding = 0
            };

            main1.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            main1.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            main1.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });
            main1.ColumnDefinitions.Add(new ColumnDefinition { Width = GridLength.Star });

            main1.RowDefinitions.Add(new RowDefinition { Height = 40 });

            var pointText = new Label
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Start,
                Text = "Punto",
                BackgroundColor = Color.Black,
                TextColor = Color.White
            };

            Grid.SetColumn(pointText, 0);
            Grid.SetRow(pointText, 0);

            main1.Children.Add(pointText);

            var xText = new Label
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "X",
                BackgroundColor = Color.Black,
                TextColor = Color.White
            };

            Grid.SetColumn(xText, 1);
            Grid.SetRow(xText, 0);

            main1.Children.Add(xText);

            var yText = new Label
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "Y",
                BackgroundColor = Color.Black,
                TextColor = Color.White
            };

            Grid.SetColumn(yText, 2);
            Grid.SetRow(yText, 0);

            main1.Children.Add(yText);

            var timeText = new Label
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                Text = "Tiempo",
                BackgroundColor = Color.Black,
                TextColor = Color.White
            };

            Grid.SetColumn(timeText, 3);
            Grid.SetRow(timeText, 0);

            main1.Children.Add(timeText);

            var tiempoarestar = mytest.Points[1].Time;

            for (var i = 1; i < mytest.Points.Count(); i++)
            {

                mytest.Points[i].Time = mytest.Points[i].Time - tiempoarestar;
            }

            foreach (PointClass punto in mytest.Points)
            {
                i++;

                string texto = "";

                if (i == 1)
                    texto = "Anclaje";
                else if (i == 2)
                    texto = "Inicio";
                else if (i == mytest.Points.Count)
                    texto = "Fin";
                else
                    texto = "Punto " + (i - 2);

                main1.RowDefinitions.Add(new RowDefinition { Height = 40 });

                pointText = new Label
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    HorizontalTextAlignment = TextAlignment.Start,
                    Text = texto,
                    BackgroundColor = Color.White,
                    TextColor = Color.Black
                };

                Grid.SetColumn(pointText, 0);
                Grid.SetRow(pointText, i);

                main1.Children.Add(pointText);

                xText = new Label
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    HorizontalTextAlignment = TextAlignment.Center,
                    Text = "" + Decimal.Round((decimal)punto.X, 2),
                    BackgroundColor = Color.White,
                    TextColor = Color.Black
                };

                Grid.SetColumn(xText, 1);
                Grid.SetRow(xText, i);

                main1.Children.Add(xText);

                yText = new Label
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    HorizontalTextAlignment = TextAlignment.Center,
                    Text = "" + Decimal.Round((decimal)punto.Y, 2),
                    BackgroundColor = Color.White,
                    TextColor = Color.Black
                };

                Grid.SetColumn(yText, 2);
                Grid.SetRow(yText, i);

                main1.Children.Add(yText);

                timeText = new Label
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    HorizontalTextAlignment = TextAlignment.Center,
                    Text = "" + Decimal.Round((decimal)punto.Time, 2),
                    BackgroundColor = Color.White,
                    TextColor = Color.Black
                };

                Grid.SetColumn(timeText, 3);
                Grid.SetRow(timeText, i);

                main1.Children.Add(timeText);

            }

            return main1;
        }

        public Grid SetResult()
        {
            i = 0;
            var main2 = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent,
                ColumnSpacing = 2,
                Padding = 0

            };

            main2.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            main2.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            main2.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            main2.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });
            main2.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(2, GridUnitType.Star) });

            main2.RowDefinitions.Add(new RowDefinition { Height = 40 });

            var pointGrid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent
            };

            var pointImage = new Image
            {
                Source = "btnsmall1.png",
                Aspect = Aspect.Fill
            };

            var pointText = new Label
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Text = "Punto",
                BackgroundColor = Color.Transparent,
                TextColor = Color.White,
                FontSize = 12
            };

            pointGrid.Children.Add(pointImage);
            pointGrid.Children.Add(pointText);

            Grid.SetColumn(pointGrid, 0);
            Grid.SetRow(pointGrid, 0);

            main2.Children.Add(pointGrid);

            var anguloGrid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent
            };

            var anguloImage = new Image
            {
                Source = "btnsmall2.png",
                Aspect = Aspect.Fill
            };

            var anguloText = new Label
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Text = "Ángulo (º)",
                BackgroundColor = Color.Transparent,
                TextColor = Color.White,
                FontSize = 12
            };

            anguloGrid.Children.Add(anguloImage);
            anguloGrid.Children.Add(anguloText);

            Grid.SetColumn(anguloGrid, 1);
            Grid.SetRow(anguloGrid, 0);

            main2.Children.Add(anguloGrid);

            var velocidadGrid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent
            };

            var velocidadImage = new Image
            {
                Source = "btnsmall1.png",
                Aspect = Aspect.Fill
            };

            var velocidadText = new Label
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Text = "Velocidad (rad/s)",
                BackgroundColor = Color.Transparent,
                TextColor = Color.White,
                FontSize = 12
            };

            velocidadGrid.Children.Add(velocidadImage);
            velocidadGrid.Children.Add(velocidadText);

            Grid.SetColumn(velocidadGrid, 2);
            Grid.SetRow(velocidadGrid, 0);

            main2.Children.Add(velocidadGrid);

            var aceleracionGrid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent
            };

            var aceleracionImage = new Image
            {
                Source = "btnsmall2.png",
                Aspect = Aspect.Fill
            };

            var aceleracionText = new Label
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Text = "Aceleración (rad/s\xB2)",
                BackgroundColor = Color.Transparent,
                TextColor = Color.White,
                FontSize = 12
            };

            aceleracionGrid.Children.Add(aceleracionImage);
            aceleracionGrid.Children.Add(aceleracionText);

            Grid.SetColumn(aceleracionGrid, 3);
            Grid.SetRow(aceleracionGrid, 0);

            main2.Children.Add(aceleracionGrid);

            var tiempoGrid = new Grid
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                BackgroundColor = Color.Transparent
            };

            var tiempoImage = new Image
            {
                Source = "btnsmall1.png",
                Aspect = Aspect.Fill
            };

            var tiempoText = new Label
            {
                VerticalOptions = LayoutOptions.FillAndExpand,
                HorizontalOptions = LayoutOptions.FillAndExpand,
                HorizontalTextAlignment = TextAlignment.Center,
                VerticalTextAlignment = TextAlignment.Center,
                Text = "Tiempo (s)",
                BackgroundColor = Color.Transparent,
                TextColor = Color.White,
                FontSize = 13
            };

            tiempoGrid.Children.Add(tiempoImage);
            tiempoGrid.Children.Add(tiempoText);

            Grid.SetColumn(tiempoGrid, 4);
            Grid.SetRow(tiempoGrid, 0);

            main2.Children.Add(tiempoGrid);

            var tiempoarestar = mytest2.Points[1].Time;

            for (var i = 1; i < mytest2.Points.Count(); i++)
            {

                mytest2.Points[i].Time = mytest2.Points[i].Time - tiempoarestar;
            }

            PointClass anclaje = mytest2.Points[0];
            PointClass inicio = mytest2.Points[1];
            PointClass vector_inicio = new PointClass { X = inicio.X - anclaje.X, Y = inicio.Y - anclaje.Y, Time = inicio.Time };
            PointClass vector_punto_anterior = vector_inicio;
            PointClass vector_punto = vector_inicio;

            double producto = 0;
            double modulo_inicio = Math.Sqrt(vector_inicio.X * vector_inicio.X + vector_inicio.Y * vector_inicio.Y);
            double modulo_punto = 0;
            double modulo_punto_anterior = 0;
            double angulo = 0;
            double angulo_anterior = 0;
            double velocidad = 0;
            double velocidad_anterior = 0;
            double aceleracion = 0;
            double instante_medio = 0;
            double instante_medio_anterior = 0;
            double potencia = 0;

            double icharge = Convert.ToDouble(mytest.Charge);
            double iheight = Convert.ToDouble(mytest.Height) / 100;
            double iweight = Convert.ToDouble(mytest.Weight);
            double ipheight = Convert.ToDouble(mytest.TypeTest.HeightPerCent) * iheight;
            double ipweight = Convert.ToDouble(mytest.TypeTest.WeightPerCent) * iweight;

            var box = new BoxView();

            var series = new LineSeries();


            for (var j = 2; j < mytest2.Points.Count(); j++)
            {
                i++;

                main2.RowDefinitions.Add(new RowDefinition { Height = 20 });

                vector_punto_anterior = vector_punto;
                angulo_anterior = angulo;
                velocidad_anterior = velocidad;
                instante_medio_anterior = instante_medio;

                vector_punto = new PointClass { X = mytest2.Points[j].X - anclaje.X, Y = mytest2.Points[j].Y - anclaje.Y, Time = mytest2.Points[j].Time };

                producto = vector_inicio.X * vector_punto.X + vector_inicio.Y * vector_punto.Y;

                modulo_punto = Math.Sqrt(vector_punto.X * vector_punto.X + vector_punto.Y * vector_punto.Y);

                modulo_punto_anterior = Math.Sqrt(vector_punto_anterior.X * vector_punto_anterior.X + vector_punto_anterior.Y * vector_punto_anterior.Y);

                var cos = producto / (modulo_punto_anterior * modulo_punto);

                angulo = Math.Acos(cos) * 180 / Math.PI;

                var angulorad = angulo * 0.01745;

                angulototal = decimal.Round((decimal)angulo, 2, MidpointRounding.AwayFromZero) + " º / " + decimal.Round((decimal)angulorad, 2, MidpointRounding.AwayFromZero) + " rad ";

                if (j < mytest2.Points.Count() - 1)
                    velocidad = ((angulo - angulo_anterior) * 0.01745) / (vector_punto.Time - vector_punto_anterior.Time);
                else
                    velocidad = 0;

                instante_medio = (vector_punto.Time + vector_punto_anterior.Time) / 2;

                aceleracion = (velocidad - velocidad_anterior) / (instante_medio - instante_medio_anterior);

                potencia = ((6 * icharge + 3 * ipweight) * 9.81 * Math.Sin(angulo * Math.PI / 180) + (6 * icharge + 2 * ipweight) * ipheight * iheight * aceleracion) * velocidad;

                potencia_media = potencia_media + Convert.ToDecimal(potencia > 0 ? potencia : 0);

                string texto = "";

                if (j < mytest2.Points.Count - 1)
                    texto = "P" + (j - 1);
                else
                    texto = "Fin";

                pointText = new Label
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    Text = texto,
                    BackgroundColor = Color.Transparent,
                    TextColor = Color.Black,
                    FontSize = 13
                };

                Grid.SetColumn(pointText, 0);
                Grid.SetRow(pointText, i);

                main2.Children.Add(pointText);

                anguloText = new Label
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    Text = "" + Decimal.Round((decimal)angulo, 2),
                    BackgroundColor = Color.Transparent,
                    TextColor = Color.Black,
                    FontSize = 13
                };

                Grid.SetColumn(anguloText, 1);
                Grid.SetRow(anguloText, i);

                main2.Children.Add(anguloText);

                velocidadText = new Label
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    Text = "" + Decimal.Round((decimal)velocidad, 2),
                    BackgroundColor = Color.Transparent,
                    TextColor = Color.Black,
                    FontSize = 13
                };

                Grid.SetColumn(velocidadText, 2);
                Grid.SetRow(velocidadText, i);

                main2.Children.Add(velocidadText);

                aceleracionText = new Label
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    Text = "" + Decimal.Round((decimal)aceleracion, 2),
                    BackgroundColor = Color.Transparent,
                    TextColor = Color.Black,
                    FontSize = 13
                };

                Grid.SetColumn(aceleracionText, 3);
                Grid.SetRow(aceleracionText, i);

                main2.Children.Add(aceleracionText);

                tiempoText = new Label
                {
                    VerticalOptions = LayoutOptions.FillAndExpand,
                    HorizontalOptions = LayoutOptions.FillAndExpand,
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    Text = "" + Decimal.Round((decimal)instante_medio, 3),
                    BackgroundColor = Color.Transparent,
                    TextColor = Color.Black,
                    FontSize = 13
                };

                Grid.SetColumn(tiempoText, 4);
                Grid.SetRow(tiempoText, i);

                main2.Children.Add(tiempoText);

                i++;

                main2.RowDefinitions.Add(new RowDefinition { Height = 1 });

                box = new BoxView
                {
                    BackgroundColor = Color.Black,
                    HorizontalOptions = LayoutOptions.FillAndExpand
                };

                Grid.SetRow(box, i);
                Grid.SetColumn(box, 0);
                Grid.SetColumnSpan(box, 5);

                main2.Children.Add(box);

                serievelocidad.Points.Add(new DataPoint(instante_medio, velocidad));
                serieaceleracion.Points.Add(new DataPoint(instante_medio, aceleracion));

            }

            velocidad_media = Convert.ToDecimal(angulo * 0.01745 / vector_punto.Time);
            potencia_media = potencia_media / (mytest2.Points.Count() - 2);

            return main2;
        }

        public async Task SaveTest()
        {
            var datatest = new Dictionary<string, string>();
            var datapoints = new Dictionary<string, string>();
            var pointObj = new PointClass();

            datatest["fellow_id"] = mytest.Fellow.Id;
            datatest["typetest_id"] = mytest.TypeTest.Id;
            datatest["charge"] = mytest.Charge;
            datatest["leftright"] = mytest.Leftright;
            datatest["height"] = mytest.Height;
            datatest["weight"] = mytest.Weight;

            string urlrequest = "http://appws.kinematiclab.org/api/tests/store";

            HttpClient httpClient = new HttpClient();

            var content = new FormUrlEncodedContent(datatest);

            httpClient.Timeout = TimeSpan.FromSeconds(15);
            httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            try
            {
                var response = await httpClient.PostAsync(new Uri(urlrequest), content);
                var resp = response.Content.ReadAsStringAsync().Result;

                testObj = JsonConvert.DeserializeObject<TestClass>(resp);

                if (!string.IsNullOrEmpty(testObj.Id))
                {
                    urlrequest = "http://appws.kinematiclab.org/api/points/store";

                    foreach (PointClass point in mytest.Points)
                    {
                        datapoints = new Dictionary<string, string>
                        {
                            ["test_id"] = testObj.Id,
                            ["time"] = Convert.ToString(point.Time),
                            ["x"] = Convert.ToString(point.X),
                            ["y"] = Convert.ToString(point.Y)
                        };

                        HttpClient httpClient2 = new HttpClient();

                        var content2 = new FormUrlEncodedContent(datapoints);

                        try
                        {
                            var response2 = await httpClient2.PostAsync(new Uri(urlrequest), content2);
                            var resp2 = response2.Content.ReadAsStringAsync().Result;

                            pointObj = JsonConvert.DeserializeObject<PointClass>(resp2);
                        }
                        catch (Exception ex)
                        {
                            Debug.WriteLine("MENSAJE:  " + ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("MENSAJE:  " + ex.Message);
                //await Application.Current.MainPage.DisplayAlert("¡Atención!", "Error de conexión al sistema", "OK");
                testObj = new TestClass();
            }

        }

        public void HomeAction(object sender, EventArgs ea)
        {
            Application.Current.MainPage = new NavigationPage(new Home());
        }

        public async void FellowAction(object sender, EventArgs ea)
        {
            var existingPages = Navigation.NavigationStack.ToList();
            int jj = 0;
            foreach (var page in existingPages)
            {
                if (jj  > 1)
                    Navigation.RemovePage(page);
                jj++;
            }

            await Navigation.PushAsync(new FellowDetails(mytest.Fellow.Id));
        }
    }
}