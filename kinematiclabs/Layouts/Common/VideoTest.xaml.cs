using System;
using Xamarin.Forms;
using System.Collections.Generic;
using SkiaSharp;
using SkiaSharp.Views.Forms;
using Plugin.GoogleAnalytics;

namespace kinematiclabs
{
    public partial class VideoTest : ContentPage
    {
        public PointClass anclaje;
        public PointClass inicio;
        public List<PointClass> puntos;
        public List<PointClass> finalpuntos;
        public int seccionActiva;
        public TestClass mytest;
        public List<PanContainer> panContainers;

        public VideoTest(TestClass test)
        {
            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            mytest = test;

            Reiniciar();

            if (!String.IsNullOrWhiteSpace(mytest.Video))
            {
                videoPlayer.Source = new FileVideoSource
                {
                    File = mytest.Video
                };
            }

        }

        public void Reiniciar()
        {
            puntos = new List<PointClass>();
            anclaje = null;
            inicio = null;

            accionLabel.BackgroundColor = Color.Red;
            Grid.SetColumnSpan(accionLabel, 2);
            accionLabel.Text = "SITUAR EJE DE ROTACIÓN";
            addpositionsbtn.IsVisible = false;

            for (var i = rlposiciones.Children.Count - 1; i > 3; i--)
                rlposiciones.Children.RemoveAt(i);
            panContainers = new List<PanContainer>();

            var sl = rlposiciones.Children[1] as ContentView;
            sl.Content = null;

            DeleteGrid.IsVisible = false;
            ResultadosGrid.IsVisible = false;
            AnclajeGrid.IsVisible = true;
            InicioGrid.IsVisible = false;
            GuardarpuntoGrid.IsVisible = false;

            seccionActiva = 1;

            if (panContainers.Count == 0)
            {
                var pcAnclaje = new PanContainer();

                var mySL = new StackLayout()
                {
                    BackgroundColor = Color.Transparent,
                    Padding = new Thickness(0),
                    WidthRequest = 30,
                    HorizontalOptions = LayoutOptions.Start
                };

                var myimage = new Image()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Source = "puntos_eje"
                };

                mySL.Children.Add(myimage);
                pcAnclaje.Content = mySL;

                panContainers.Add(pcAnclaje);

                rlposiciones.Children.Add(panContainers[0], Constraint.Constant(0), Constraint.Constant(0), Constraint.RelativeToParent((parent) => { return parent.Width; }), Constraint.RelativeToParent((parent) => { return parent.Height * 0.85; }));

                //Grid.SetRow(rlposiciones, 1);
                //gridPadre.Children.Add()
                //Content = rlposiciones;
            }
        }

        void OnPlayPauseButtonClicked(object sender, EventArgs args)
        {
            if (videoPlayer.Status == VideoStatus.Playing)
            {
                videoPlayer.Pause();
            }
            else if (videoPlayer.Status == VideoStatus.Paused)
            {
                videoPlayer.Play();
            }
        }

        private void PreviousClicked(object sender, EventArgs e)
        {
            if (videoPlayer.Position.TotalMilliseconds >= 33)
            {
                videoPlayer.Position = TimeSpan.FromMilliseconds(videoPlayer.Position.TotalMilliseconds - 33);
                //positionSlider.Value = (videoPlayer.Position.TotalMilliseconds - 30) / 1000;
            }

            else
            {
                videoPlayer.Position = TimeSpan.FromMilliseconds(0);
                //positionSlider.Value = 0;
            }
        }

        private void NextClicked(object sender, EventArgs e)
        {
            if (videoPlayer.Position.TotalMilliseconds <= videoPlayer.Duration.TotalMilliseconds - 33)
            {
                videoPlayer.Position = TimeSpan.FromMilliseconds(videoPlayer.Position.TotalMilliseconds + 33);
                //positionSlider.Value = (videoPlayer.Position.TotalMilliseconds + 30) / 1000;
            }
            else
            {
                videoPlayer.Position = TimeSpan.FromMilliseconds(videoPlayer.Duration.TotalMilliseconds);
                //positionSlider.Value = videoPlayer.Duration.TotalMilliseconds / 1000;
            }
        }

        public void SetAnclaje(object sender, EventArgs ea)
        {
            var pcanclaje = rlposiciones.Children[4] as PanContainer;

            var dx = pcanclaje.Content.TranslationX + 15;
            var dy = pcanclaje.Content.TranslationY + 15;
            anclaje = new PointClass { X = dx, Y = dy, Time = 0 };
            rlposiciones.Children[4].IsEnabled = false;

            var sl = rlposiciones.Children[1] as ContentView;

            if (sl.Content != null)
                sl.Content = null;

            SKCanvasView canvasView = new SKCanvasView();
            canvasView.PaintSurface += OnCanvasViewPaintSurface;

            sl.Content = canvasView;

            accionLabel.BackgroundColor = Color.FromHex("#1dd220");
            Grid.SetColumnSpan(accionLabel, 2);
            accionLabel.Text = "SITUAR INICIO";

            DeleteGrid.IsVisible = true;
            ResultadosGrid.IsVisible = false;
            AnclajeGrid.IsVisible = false;
            InicioGrid.IsVisible = true;
            GuardarpuntoGrid.IsVisible = false;

            if (panContainers.Count == 1)
            {
                var pcInicio = new PanContainer();

                var mySL = new StackLayout()
                {
                    BackgroundColor = Color.Transparent,
                    Padding = new Thickness(0),
                    WidthRequest = 30,
                    HorizontalOptions = LayoutOptions.Start
                };

                var myimage = new Image()
                {
                    HorizontalOptions = LayoutOptions.Center,
                    VerticalOptions = LayoutOptions.Center,
                    Source = "puntos_inicio"
                };

                mySL.Children.Add(myimage);
                pcInicio.Content = mySL;

                panContainers.Add(pcInicio);

                rlposiciones.Children.Add(panContainers[1], Constraint.Constant(0), Constraint.Constant(0), Constraint.RelativeToParent((parent) => { return parent.Width; }), Constraint.RelativeToParent((parent) => { return parent.Height * 0.85; }));
                //Content = rlposiciones;
            }

            seccionActiva = 2;
        }

        public async void SetInicio(object sender, EventArgs ea)
        {
            var pcinicio = rlposiciones.Children[5] as PanContainer;

            var dx = pcinicio.Content.TranslationX + 15;
            var dy = pcinicio.Content.TranslationY + 15;
            double dtime = (videoPlayer.Position.TotalMilliseconds) / 1000;
            inicio = new PointClass { X = dx, Y = dy, Time = dtime };
            if (inicio.X == anclaje.X && inicio.Y == anclaje.Y)
            {
                await Application.Current.MainPage.DisplayAlert("Atención", "El punto de anclaje y de inicio no pueden estar en la misma posición", "OK");
            }
            else
            {
                rlposiciones.Children[5].IsEnabled = false;

                var sl = rlposiciones.Children[1] as ContentView;

                if (sl.Content != null)
                    sl.Content = null;

                SKCanvasView canvasView = new SKCanvasView();
                canvasView.PaintSurface += OnCanvasViewPaintSurface;

                sl.Content = canvasView;

                accionLabel.BackgroundColor = Color.FromHex("#3a5fcd");
                Grid.SetColumnSpan(accionLabel, 1);
                addpositionsbtn.IsVisible = true;
                accionLabel.Text = "SITUAR PUNTOS";

                DeleteGrid.IsVisible = true;
                ResultadosGrid.IsVisible = false;
                AnclajeGrid.IsVisible = false;
                InicioGrid.IsVisible = false;
                GuardarpuntoGrid.IsVisible = false;

                seccionActiva = 3;
            }

        }

        public void AddPoint(object sender, EventArgs ea)
        {
            addpositionsbtn.IsEnabled = false;
            GuardarpuntoGrid.IsVisible = true;



            var pcOtro = new PanContainer(anclaje, inicio);

            var mySL = new StackLayout()
            {
                BackgroundColor = Color.Transparent,
                Padding = new Thickness(0),
                WidthRequest = 30,
                HorizontalOptions = LayoutOptions.Start,
                Spacing = 0
            };

            var myimage = new Image()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                Source = "puntos_intermedios"
            };
            var mylabel = new Label()
            {
                HorizontalOptions = LayoutOptions.Center,
                VerticalOptions = LayoutOptions.Center,
                TextColor = Color.White,
                FontSize = 10
            };

            mySL.Children.Add(myimage);
            mySL.Children.Add(mylabel);
            pcOtro.Content = mySL;

            panContainers.Add(pcOtro);

            rlposiciones.Children.Add(panContainers[panContainers.Count - 1], Constraint.Constant(0), Constraint.Constant(0), Constraint.RelativeToParent((parent) => { return parent.Width; }), Constraint.RelativeToParent((parent) => { return parent.Height * 0.85; }));
            //Content = rlposiciones;
        }

        public void SavePoint(object sender, EventArgs ea)
        {
            addpositionsbtn.IsEnabled = true;
            GuardarpuntoGrid.IsVisible = false;

            if (panContainers.Count > 2)
            {
                var pcgrabar = rlposiciones.Children[rlposiciones.Children.Count - 1] as PanContainer;

                var dx = pcgrabar.Content.TranslationX + 15;
                var dy = pcgrabar.Content.TranslationY + 15;
                double dtime = (videoPlayer.Position.TotalMilliseconds) / 1000;
                puntos.Add(new PointClass { X = dx, Y = dy, Time = dtime });
                rlposiciones.Children[rlposiciones.Children.Count - 1].IsEnabled = false;

                var sl = rlposiciones.Children[1] as ContentView;

                if (sl.Content != null)
                    sl.Content = null;

                SKCanvasView canvasView = new SKCanvasView();
                canvasView.PaintSurface += OnCanvasViewPaintSurface;

                sl.Content = canvasView;

                //if (puntos.Count > 1)
                ResultadosGrid.IsVisible = true;
            }

        }


        void OnCanvasViewPaintSurface(object sender, SKPaintSurfaceEventArgs args)
        {
            SKImageInfo info = args.Info;
            SKSurface surface = args.Surface;
            SKCanvas canvas = surface.Canvas;

            var sl = rlposiciones.Children[1] as ContentView;
            var fwidth = info.Width / sl.Width;
            var fheight = info.Height / sl.Height;

            canvas.Clear();

            SKPaint paint = new SKPaint
            {
                Style = SKPaintStyle.Stroke,
                Color = Color.Yellow.ToSKColor(),
                StrokeWidth = 3,
                StrokeCap = SKStrokeCap.Round,
                PathEffect = SKPathEffect.CreateDash(new[] { (float)10, (float)10 }, 10)
            };

            if (inicio != null)
                canvas.DrawLine((float)(anclaje.X * fwidth), (float)(anclaje.Y * fheight), (float)(inicio.X * fwidth), (float)(inicio.Y * fheight), paint);

            foreach (var punto in puntos)
                canvas.DrawLine((float)(anclaje.X * fwidth), (float)(anclaje.Y * fheight), (float)(punto.X * fwidth), (float)(punto.Y * fheight), paint);
        }

        public async void DeletePosiciones(object sender, EventArgs ea)
        {
            var response = await Application.Current.MainPage.DisplayAlert("Atención", "¿Está seguro de que quiere reiniciar las posiciones marcadas?", "Si", "No");
            if (response)
            {
                Reiniciar();
            }
        }

        public async void Resultados(object sender, EventArgs ea)
        {
            var response = await Application.Current.MainPage.DisplayAlert("Atención", "Si decide ver los resultados, no podrá modificar este test, ¿está seguro de que quiere continuar?", "Si", "No");
            if (response)
            {
                /*var pcgrabar = rlposiciones.Children[rlposiciones.Children.Count - 1] as PanContainer;

                var dx = pcgrabar.Content.TranslationX + 15;
                var dy = pcgrabar.Content.TranslationY + 15;
                double dtime = (videoPlayer.Position.TotalMilliseconds) / 1000;
                puntos.Add(new PointClass { X = dx, Y = dy, Time = dtime });
                rlposiciones.Children[rlposiciones.Children.Count - 1].IsEnabled = false;

                var sl = rlposiciones.Children[1] as ContentView;

                if (sl.Content != null)
                    sl.Content = null;

                SKCanvasView canvasView = new SKCanvasView();
                canvasView.PaintSurface += OnCanvasViewPaintSurface;

                sl.Content = canvasView;*/

                finalpuntos = new List<PointClass>();
                finalpuntos.Add(anclaje);
                finalpuntos.Add(inicio);

                foreach (PointClass punto in puntos)
                    finalpuntos.Add(punto);

                finalpuntos.Sort((p, q) => p.Time.CompareTo(q.Time));

                mytest.Points = finalpuntos;

                var hayerrores = false;
                for (var i = 2; i < finalpuntos.Count; i++)
                    if (finalpuntos[i].Time <= finalpuntos[i - 1].Time)
                        hayerrores = true;
                if (hayerrores)
                    await Application.Current.MainPage.DisplayAlert("Atención", "Hay posiciones con el mismo instante de tiempo y no se podrán realizar los cálculos", "OK");
                else
                    await Navigation.PushAsync(new TestResults(mytest));
            }
        }

        public void HomeAction(object sender, EventArgs ea)
        {
            Application.Current.MainPage = new NavigationPage(new Home());
        }

        public async void BackAction(object sender, EventArgs ea)
        {
            await Navigation.PopAsync();
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            GoogleAnalytics.Current.Tracker.SendView("VideoTest");
        }
    }
}
