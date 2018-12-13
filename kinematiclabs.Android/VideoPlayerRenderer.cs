using System;
using System.ComponentModel;
using System.IO;
using Android.Content;
using Android.Widget;
using ARelativeLayout = Android.Widget.RelativeLayout;
using Xamarin.Forms;
using Xamarin.Forms.Platform.Android;
using kinematiclabs;
using kinematiclabs.Droid;

[assembly: ExportRenderer(typeof(VideoPlayer),
                          typeof(VideoPlayerRenderer))]

namespace kinematiclabs.Droid
{
    public class VideoPlayerRenderer : ViewRenderer<VideoPlayer, ARelativeLayout>
    {
        VideoView videoView;
        MediaController mediaController;    // Used to display transport controls
        bool isPrepared;

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

                    videoView.CanSeekBackward();
                    videoView.CanSeekForward();
                    // Put the VideoView in a RelativeLayout
                    ARelativeLayout relativeLayout = new ARelativeLayout(Context);
                    relativeLayout.AddView(videoView);

                    // Center the VideoView in the RelativeLayout
                    ARelativeLayout.LayoutParams layoutParams =
                        new ARelativeLayout.LayoutParams(LayoutParams.MatchParent, LayoutParams.MatchParent);
                    layoutParams.AddRule(LayoutRules.CenterInParent);
                    videoView.LayoutParameters = layoutParams;

                    // Handle a VideoView event
                    videoView.Prepared += OnVideoViewPrepared;

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
            if (Control != null && videoView != null)
            {
                videoView.Prepared -= OnVideoViewPrepared;
            }
            if (Element != null)
            {
                Element.UpdateStatus -= OnUpdateStatus;
            }

            base.Dispose(disposing);
        }

        void OnVideoViewPrepared(object sender, EventArgs args)
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

                var zz = Element.Position.TotalMilliseconds - videoView.CurrentPosition;

                if (Math.Abs(zz) >= 30)
                {
                    //((IElementController)Element).SetValueFromRenderer(VideoPlayer.PositionProperty, TimeSpan.FromMilliseconds(videoView.CurrentPosition + zz));
                    int xc = (int)Element.Position.TotalMilliseconds;
                    videoView.SeekTo(xc);

                }
                else if (Element.Position.TotalMilliseconds >= videoView.Duration)
                {
                    videoView.SeekTo(videoView.Duration);
                }
                else if (Element.Position.TotalMilliseconds <= 0)
                {
                    videoView.SeekTo(0);
                }
                ((IElementController)Element).SetValueFromRenderer(VideoPlayer.PositionProperty, TimeSpan.FromMilliseconds(Element.Position.TotalMilliseconds));
            }
        }

        void SetAreTransportControlsEnabled()
        {
            if (Element.AreTransportControlsEnabled)
            {
                mediaController = new MediaController(Context, false);
                mediaController.SetMediaPlayer(videoView);
                videoView.SetMediaController(mediaController);

                mediaController.NextClick += MediaController_NextClick;
                mediaController.PreviousClick += MediaController_PreviousClick;
                mediaController.Show();
            }
            else
            {
                videoView.SetMediaController(null);

                if (mediaController != null)
                {
                    mediaController.SetMediaPlayer(null);
                    mediaController = null;
                }
            }
        }

        private void MediaController_PreviousClick(object sender, EventArgs e)
        {
            if (TimeSpan.FromMilliseconds(videoView.CurrentPosition).TotalMilliseconds > 30)
                videoView.SeekTo(videoView.CurrentPosition - 30);
            //((IElementController)Element).SetValueFromRenderer(VideoPlayer.PositionProperty, TimeSpan.FromMilliseconds(videoView.CurrentPosition - 100));
        }

        private void MediaController_NextClick(object sender, EventArgs e)
        {
            if (TimeSpan.FromMilliseconds(videoView.CurrentPosition).TotalMilliseconds < videoView.Duration - 30)
                videoView.SeekTo(videoView.CurrentPosition + 30);
                //((IElementController)Element).SetValueFromRenderer(VideoPlayer.PositionProperty, TimeSpan.FromMilliseconds(videoView.CurrentPosition + 100));
        }

        void SetSource()
        {
            isPrepared = false;
            bool hasSetSource = false;

            if (Element.Source is FileVideoSource)
            {
                string filename = (Element.Source as FileVideoSource).File;

                if (!String.IsNullOrWhiteSpace(filename))
                {
                    videoView.SetVideoPath(filename);
                    hasSetSource = true;
                }
            }
            else if (Element.Source is ResourceVideoSource)
            {
                string package = Context.PackageName;
                string path = (Element.Source as ResourceVideoSource).Path;

                if (!String.IsNullOrWhiteSpace(path))
                {
                    string filename = Path.GetFileNameWithoutExtension(path).ToLowerInvariant();
                    string uri = "android.resource://" + package + "/raw/" + filename;
                    videoView.SetVideoURI(Android.Net.Uri.Parse(uri));
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
    }

}
 