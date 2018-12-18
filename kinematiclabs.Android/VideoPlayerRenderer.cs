using System;
using System.ComponentModel;
using Android.Content;
using Android.Util;
using Itequia.Controls.MediaPlayer;
using kinematiclabs;
using kinematiclabs.Droid;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using static Itequia.Controls.MediaPlayer.MediaPlayer;
using ARelativeLayout = Android.Widget.RelativeLayout;
using MediaController = Android.Widget.MediaController;

[assembly: ExportRenderer(typeof(VideoPlayer),
                          typeof(VideoPlayerRenderer))]

namespace kinematiclabs.Droid
{
    public class VideoPlayerRenderer : ViewRenderer<VideoPlayer, ARelativeLayout>, IOnErrorListener, IOnPreparedListener
    {
        private const string LogTag = "Itequia";

        VideoView videoView;
        MediaController mediaController;    // Used to display transport controls
        IMediaSource mediaSource;
        bool isPrepared;
        private MediaPlayer _mediaPlayer;

        public VideoPlayerRenderer(Context context) : base(context)
        {
        }

        protected override void OnElementChanged(ElementChangedEventArgs<VideoPlayer> e)
        {
            base.OnElementChanged(e);

            if (e.NewElement != null)
            {
                if (Control == null)
                {
                    // Save the VideoView for future reference
                    videoView = new VideoView(Context);

                    videoView.SetOnErrorListener(this);
                    videoView.SetOnPreparedListener(this);

                    videoView.CanSeekBackward();
                    videoView.CanSeekForward();
                    // Put the VideoView in a RelativeLayout
                    ARelativeLayout relativeLayout = new ARelativeLayout(Context);
                    relativeLayout.AddView(videoView);

                    // Center the VideoView in the RelativeLayout
                    ARelativeLayout.LayoutParams layoutParams =
                        new ARelativeLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                    layoutParams.AddRule(Android.Widget.LayoutRules.CenterInParent);
                    videoView.LayoutParameters = layoutParams;

                    SetNativeControl(relativeLayout);

                }

                SetAreTransportControlsEnabled();
                SetSource();

                e.NewElement.UpdateStatus += OnUpdateStatus;
                e.NewElement.PlayRequested += OnPlayRequested;
                e.NewElement.PauseRequested += OnPauseRequested;
                e.NewElement.StopRequested += OnStopRequested;

            }

            if (e.OldElement != null)
            {
                e.OldElement.UpdateStatus -= OnUpdateStatus;
                e.OldElement.PlayRequested -= OnPlayRequested;
                e.OldElement.PauseRequested -= OnPauseRequested;
                e.OldElement.StopRequested -= OnStopRequested;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (Element != null)
            {
                Element.UpdateStatus -= OnUpdateStatus;
            }

            base.Dispose(disposing);
        }

        void OnVideoViewPrepared(MediaPlayer mediaPlayer, EventArgs args)
        {
            isPrepared = true;
            ((IVideoPlayerController)Element).Duration = TimeSpan.FromMilliseconds(videoView.Duration);
        }

        protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            base.OnElementPropertyChanged(sender, e);

            if (e.PropertyName == VideoPlayer.AreTransportControlsEnabledProperty.PropertyName)
            {
                SetAreTransportControlsEnabled();
            }
            else if (e.PropertyName == VideoPlayer.SourceProperty.PropertyName)
            {
                SetSource();
            }
            else if (e.PropertyName == VideoPlayer.PositionProperty.PropertyName)
            {
                OnPositionChanged();
            }
        }

        private void OnPositionChanged()
        {
            int? millisToSeek = null;
            var millisDifferent = System.Math.Abs(Element.Position.TotalMilliseconds - videoView.CurrentPosition);

            if (millisDifferent >= 30)
            {
                millisToSeek = (int)Element.Position.TotalMilliseconds;
            }
            else if (Element.Position.TotalMilliseconds >= videoView.Duration)
            {
                millisToSeek = videoView.Duration;
            }
            else if (Element.Position.TotalMilliseconds <= 0)
            {
                millisToSeek = 0;
            }

            if (millisToSeek != null && millisToSeek.HasValue && millisToSeek != -1)
            {
                Log.Debug(LogTag, $"Millis to seek {millisToSeek.Value} of total {(int)Element.Duration.TotalMilliseconds}");
                SeekTo(millisToSeek.Value);
            }

            TimeSpan timeSpan = TimeSpan.FromMilliseconds(Element.Position.TotalMilliseconds);
            ((IElementController)Element).SetValueFromRenderer(VideoPlayer.PositionProperty, timeSpan);
        }

        void SetAreTransportControlsEnabled()
        {
            try
            {
                if (Element.AreTransportControlsEnabled)
                {
                    mediaController = new MediaController(Context, false);
                    mediaController.SetMediaPlayer(videoView);
                    mediaController.Enabled = true;
                    mediaController.Show();
                }
                else
                {
                    if (mediaController != null){
                        mediaController.SetMediaPlayer(null);
                        mediaController = null;
                    }
                }
            }
            catch (System.Exception ex)
            {
                Log.Error(LogTag, $"{ex.Message}\n{ex.StackTrace}");
            }
        }

        private void SetSource()
        {
            isPrepared = false;
            var hasSetSource = false;

            var filePath = (Element.Source as FileVideoSource).File;

            if (Element.Source is FileVideoSource)
            {

                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    mediaSource = new FileSource(new Java.IO.File(filePath));
                    videoView.SetVideoSource(mediaSource);
                    hasSetSource = true;
                }
            }
            else if (Element.Source is ResourceVideoSource)
            {
                string package = Context.PackageName;
                string path = (Element.Source as ResourceVideoSource).Path;

                if (!string.IsNullOrWhiteSpace(path))
                {
                    string filename = System.IO.Path.GetFileNameWithoutExtension(path).ToLowerInvariant();
                    string uri = "android.resource://" + package + "/raw/" + filename;
                    mediaSource = new UriSource(Context, Android.Net.Uri.Parse(uri));

                    hasSetSource = true;
                }
            }

            if (hasSetSource && Element.AutoPlay)
            {
                videoView.Start();
            }
        }

        // Event handler to update status
        void OnUpdateStatus(object sender, EventArgs args)
        {
            VideoStatus status = VideoStatus.NotReady;

            if (isPrepared)
            {
                status = videoView.IsPlaying ? VideoStatus.Playing : VideoStatus.Paused;
            }

            ((IVideoPlayerController)Element).Status = status;

            // Set Position property
            TimeSpan timeSpan = TimeSpan.FromMilliseconds(videoView.CurrentPosition);
            ((IElementController)Element).SetValueFromRenderer(VideoPlayer.PositionProperty, timeSpan);
        }

        // Event handlers to implement methods
        void OnPlayRequested(object sender, EventArgs args)
        {
            videoView.Start();
        }

        void OnPauseRequested(object sender, EventArgs args)
        {
            videoView.Pause();
        }

        void OnStopRequested(object sender, EventArgs args)
        {
            videoView.StopPlayback();
        }

        public void SeekTo(double millisToSeek)
        {
            int convertedMillis = (int)Math.Abs(millisToSeek);

            Log.Debug(LogTag, $"Seeking {convertedMillis} millis.");

            _mediaPlayer.SeekTo(convertedMillis);
        }

        public void OnPrepared(MediaPlayer mp)
        {
            OnVideoViewPrepared(mp, EventArgs.Empty);
            _mediaPlayer = mp;
            _mediaPlayer.SetSeekMode(SeekMode.Precise);
        }

        public bool OnError(MediaPlayer mp, int p1, int p2)
        {
            Log.Error(LogTag, $"There was an error with MediaPlayer");

            return true;
        }
    }
}
