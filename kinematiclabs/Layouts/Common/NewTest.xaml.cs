using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;
using System.Diagnostics;
using Xamarin.Forms;
using System.Threading.Tasks;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.GoogleAnalytics;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;

namespace kinematiclabs
{
    public partial class NewTest : ContentPage
    {
        public FellowClass fellowObj;
        public UserClass userObj;
        public List<TypeTestClass> typesList;
        public List<FamilyTestClass> familiesList;
        public List<TypeTestClass> typesOC { get; set; }
        public List<FamilyTestClass> familiesOC { get; set; }
        public TestClass mynewtest;
        public TypeTestClass mytypetest;

        public NewTest(FellowClass ifellow)
        {
            userObj = Application.Current.Properties["user"] as UserClass;
            fellowObj = ifellow;

            InitializeComponent();

            NavigationPage.SetHasNavigationBar(this, false);

            SetPickers();

            familyPicker.SelectedIndexChanged += HandlefamilyPickerSelectedIndexChanged;
            zonePicker.SelectedIndexChanged += HandlezonePickerSelectedIndexChanged;
            typePicker.SelectedIndexChanged += HandletypePickerSelectedIndexChanged;

            if (fellowObj != null && fellowObj.Id != "0")
            {
                Task.Run(GetTypes).Wait();
                Task.Run(GetFamilies).Wait();

                zonePicker.SelectedIndex = 0;
            }
        }

        public async Task GetTypes()
        {
            HttpClient httpClient = new HttpClient();
            string urlrequest = "";
            if (fellowObj.Sex == "Hombre")
            {
                urlrequest = "http://appws.kinematiclab.org/api/typesbysex/0";
            }
            else
            {
                urlrequest = "http://appws.kinematiclab.org/api/typesbysex/1";
            }

            httpClient.Timeout = TimeSpan.FromSeconds(15);

            try
            {
                var response = await httpClient.GetAsync(new Uri(urlrequest));

                var resp = response.Content.ReadAsStringAsync().Result;

                typesList = JsonConvert.DeserializeObject<List<TypeTestClass>>(resp);
            }
            catch (Exception e)
            {
                Debug.WriteLine("MENSAJE:  " + e.Message);
            }
        }

        public async Task GetFamilies()
        {
            HttpClient httpClient = new HttpClient();
            string urlrequest = "http://appws.kinematiclab.org/api/familytests";

            httpClient.Timeout = TimeSpan.FromSeconds(15);

            try
            {
                var response = await httpClient.GetAsync(new Uri(urlrequest));

                var resp = response.Content.ReadAsStringAsync().Result;

                familiesList = JsonConvert.DeserializeObject<List<FamilyTestClass>>(resp);
            }
            catch (Exception e)
            {
                Debug.WriteLine("MENSAJE:  " + e.Message);
            }
        }

        public void SetPickers()
        {
            zonePicker.Items.Add("Miembro superior");
            zonePicker.Items.Add("Miembro inferior");
            zonePicker.Items.Add("General");

            for (int i = 1; i < 150; i++)
            {
                weightPicker.Items.Add("" + i);
            }
            weightPicker.SelectedIndex = 59;

            for (int i = 1; i < 250; i++)
            {
                heightPicker.Items.Add("" + i);
            }
            heightPicker.SelectedIndex = 169;

            for (int i = 0; i < 120; i++)
            {
                chargePicker.Items.Add("" + i);
            }
            chargePicker.SelectedIndex = 0;

        }

        private void HandlezonePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            if (picker.SelectedIndex > -1)
            {
                var zone = picker.Items[picker.SelectedIndex];
                familyPicker.Items.Clear();
                typePicker.Items.Clear();
                familiesOC = new List<FamilyTestClass>(familiesList);
                familiesOC.RemoveAll(x => x.Member != zone);
                foreach (FamilyTestClass f in familiesOC)
                {
                    familyPicker.Items.Add(f.Name);
                }
                familyPicker.SelectedIndex = 0;

            }
        }

        private void HandlefamilyPickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            if (picker.SelectedIndex > -1)
            {
                var family = picker.Items[picker.SelectedIndex];
                var familyID = familiesList.Find(x => x.Name == family).Id;
                typePicker.Items.Clear();
                typesOC = new List<TypeTestClass>(typesList);
                typesOC.RemoveAll(x => x.FamilyTest.Id != familyID);
                foreach (TypeTestClass t in typesOC)
                {
                    typePicker.Items.Add(t.Name);
                }
                typePicker.SelectedIndex = 0;
            }
        }

        private void HandletypePickerSelectedIndexChanged(object sender, EventArgs e)
        {
            var picker = (Picker)sender;
            if (picker.SelectedIndex > -1)
            {
                var typename = picker.Items[picker.SelectedIndex];
                var mytype = typesList.Find(x => x.Name == typename);

                segmentPicker.Items.Clear();

                if(mytype.Haslaterality == "1")
                {
                    segmentText.IsVisible = false;
                    segmentPicker.IsVisible = true;
                    if (mytype.Segment == "Brazo" || mytype.Segment == "Antebrazo" || mytype.Segment == "Pie")
                    {
                        segmentPicker.Items.Add(mytype.Segment + " derecho");
                        segmentPicker.Items.Add(mytype.Segment + " izquierdo");
                    }
                    else
                    {
                        segmentPicker.Items.Add(mytype.Segment + " derecha");
                        segmentPicker.Items.Add(mytype.Segment + " izquierda");
                    }
                    segmentPicker.SelectedIndex = 0;
                }
                else
                {
                    segmentText.IsVisible = true;
                    segmentPicker.IsVisible = false;
                    segmentText.Text = mytype.Segment;
                }

                positionText.Text = mytype.Position;
                mytypetest = mytype;
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

        public async void DemoAction(object sender, EventArgs ea)
        {
            await Navigation.PushModalAsync(new DemoVideo(mytypetest.Gif));
        }

        protected override void OnAppearing()
        {
            base.OnAppearing();

            GoogleAnalytics.Current.Tracker.SendView("NewTest");
        }

        public async void NextAction(object sender, EventArgs ea)
        {
            String path = "";
            String apath = "";
            String laterality = "Ninguna";

            var storageStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
            var photoStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Photos);
            var cameraStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Camera);
            var microStatus = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Microphone);

            if (storageStatus != PermissionStatus.Granted || photoStatus != PermissionStatus.Granted
               || cameraStatus != PermissionStatus.Granted || microStatus != PermissionStatus.Granted)
            {
                if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Location))
                {
                    await DisplayAlert("Need location", "Gunna need that location", "OK");
                }
                var results = await CrossPermissions.Current.RequestPermissionsAsync(new[] { Permission.Storage, Permission.Photos, Permission.Camera, Permission.Microphone });
                storageStatus = results[Permission.Storage];
                photoStatus = results[Permission.Photos];
                cameraStatus = results[Permission.Camera];
                microStatus = results[Permission.Microphone];
            }

            if (photoStatus == PermissionStatus.Granted && storageStatus == PermissionStatus.Granted)
            {
                await CrossMedia.Current.Initialize();

                var action = await DisplayActionSheet("Elige una opción", "Cancelar", null, "Grabar video", "Adjuntar video de la galería");

                if (action == "Grabar video")
                {
                    if (!CrossMedia.Current.IsCameraAvailable || !CrossMedia.Current.IsTakePhotoSupported)
                    {
                        await DisplayAlert("No Camera", ":( No camera available.", "OK");
                        return;
                    }

                    if (cameraStatus == PermissionStatus.Granted && microStatus == PermissionStatus.Granted)
                    {
                        var file = await CrossMedia.Current.TakeVideoAsync(new StoreVideoOptions
                        {
                            Quality = VideoQuality.High,
                            SaveToAlbum = true,
                            Directory = "KiematicFiles"
                        });

                        if (file == null)
                            return;

                        apath = file.AlbumPath;

                        path = file.Path;
                    }
                    else
                    {
                        await Application.Current.MainPage.DisplayAlert("Atencìón", "Sin los permisos adecuados (Privacidad -> Cámara, Privacidad -> Micrófono) en tu dispositivo no podemos continuar con el test", "OK");
                    }
                }
                else if (action == "Adjuntar video de la galería")
                {
                    MediaFile file = await CrossMedia.Current.PickVideoAsync();

                    if (file == null)
                        return;

                    apath = file.AlbumPath;

                    path = file.Path;
                }

                if (segmentPicker.IsVisible)
                {
                    laterality = segmentPicker.SelectedIndex == 0 ? "Derecha" : "Izquierda";
                }

                if (path != "")
                {
                    mynewtest = new TestClass
                    {
                        Height = heightPicker.Items[heightPicker.SelectedIndex],
                        Weight = weightPicker.Items[weightPicker.SelectedIndex],
                        Charge = chargePicker.Items[chargePicker.SelectedIndex],
                        Fellow = fellowObj,
                        //Video = path,
                        Video = Device.RuntimePlatform == Device.iOS ? apath : path,
                        TypeTest = mytypetest,
                        Leftright = laterality
                    };

                    await Navigation.PushAsync(new VideoTest(mynewtest));
                }
            }
            else
            {
                await Application.Current.MainPage.DisplayAlert("Atencìón", "Sin los permisos adecuados (Privacidad -> Fotos) en tu dispositivo no podemos continuar con el test", "OK");
            }
        }
    }
}
