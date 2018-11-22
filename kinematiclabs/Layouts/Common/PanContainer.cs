using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace kinematiclabs
{
    public class PanContainer : ContentView
    {
        double x, y;
        PointClass inicio;
        PointClass anclaje;

        public PanContainer()
        {
            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnPanUpdated;
            GestureRecognizers.Add(panGesture);
        }

        public PanContainer(PointClass anc, PointClass ini)
        {
            var panGesture = new PanGestureRecognizer();
            panGesture.PanUpdated += OnPanUpdated;
            GestureRecognizers.Add(panGesture);
            inicio = ini;
            anclaje = anc;
        }

        void OnPanUpdated(object sender, PanUpdatedEventArgs e)
        {
            switch (e.StatusType)
            {

                case GestureStatus.Running:
                    if (x + e.TotalX < -15)
                    {
                        Content.TranslationX = -15;
                    }
                    else
                    {
                        if (x + e.TotalX > Application.Current.MainPage.Width - 15)
                            Content.TranslationX = Application.Current.MainPage.Width - 15;
                        else
                            Content.TranslationX = x + e.TotalX;
                    }
                    if (y + e.TotalY < -15)
                    {
                        Content.TranslationY = -15;
                    }
                    else
                    {
                        if (y + e.TotalY > this.Height - 15) //Application.Current.MainPage.Height
                            Content.TranslationY = this.Height - 15; //Application.Current.MainPage.Height
                        else
                            Content.TranslationY = y + e.TotalY;
                    }
                    if (anclaje != null && inicio != null)
                    {
                        PointClass otro = new PointClass() { X = Content.TranslationX + 15, Y = Content.TranslationY + 15, Time = 0 };
                        PointClass vector_inicio = new PointClass { X = inicio.X - anclaje.X, Y = inicio.Y - anclaje.Y, Time = 0 };
                        PointClass vector_punto = new PointClass { X = otro.X - anclaje.X, Y = otro.Y - anclaje.Y, Time = 0 };
                        var producto = vector_inicio.X * vector_punto.X + vector_inicio.Y * vector_punto.Y;

                        var modulo_punto = Math.Sqrt(vector_punto.X * vector_punto.X + vector_punto.Y * vector_punto.Y);

                        var modulo_punto_inicio = Math.Sqrt(vector_inicio.X * vector_inicio.X + vector_inicio.Y * vector_inicio.Y);

                        var cos = producto / (modulo_punto_inicio * modulo_punto);

                        var angulo = Decimal.Round((decimal)(Math.Acos(cos) * 180 / Math.PI), 2);
                        StackLayout mysl = Content as StackLayout;
                        Label mytext = mysl.Children[1] as Label;
                        mytext.Text = angulo + "º";
                    }

                    break;

                case GestureStatus.Completed:

                    x = Content.TranslationX;
                    y = Content.TranslationY;

                    break;
            }
        }
    }
}