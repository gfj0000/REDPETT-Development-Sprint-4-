using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using FluidKit.Controls;
using Menafrinet.View;
using Menafrinet.ViewModel;


namespace Menafrinet.View.NavigationTransition
{
    public class NavigationPresenter : ContentPresenter
    {
        private readonly ContentPresenter oldContentPresenter;
        private readonly ContentPresenter newContentPresenter;
        private readonly TransitionPresenter transitionPresenter;
        private bool loaded;

        #region GoForward
        /// <summary>
        /// GoForward Dependency Property
        /// </summary>
        public static readonly DependencyProperty GoForwardProperty;

        /// <summary>
        /// Gets or sets the GoForward property
        /// </summary>
        public bool GoForward
        {
            get { return (bool)GetValue(GoForwardProperty); }
            set { SetValue(GoForwardProperty, value); }
        }
        #endregion

        #region Transition
        /// <summary>
        /// Transition Dependency Property
        /// </summary>
        public static readonly DependencyProperty TransitionProperty;

        /// <summary>
        /// Gets or sets the Transition property
        /// </summary>
        public Transition Transition
        {
            get { return (Transition)GetValue(TransitionProperty); }
            set { SetValue(TransitionProperty, value); }
        }
        #endregion

        internal UIElementCollection Children
        {
            get;
            private set;
        }

        static NavigationPresenter()
        {
            GoForwardProperty = DependencyProperty.Register(
                "GoForward",
                typeof(bool),
                typeof(NavigationPresenter),
                new FrameworkPropertyMetadata(false));

            TransitionProperty = DependencyProperty.Register(
                "Transition",
                typeof(Transition),
                typeof(NavigationPresenter),
                new FrameworkPropertyMetadata(new NavigationCubeTransition(), new PropertyChangedCallback(OnTransitionChanged)));

            ContentProperty.OverrideMetadata(
                typeof(NavigationPresenter),
                new FrameworkPropertyMetadata(null, new PropertyChangedCallback(OnContentValueChanged)));
        }

        public NavigationPresenter()
        {
            this.oldContentPresenter = new ContentPresenter();
            this.newContentPresenter = new ContentPresenter();

            this.transitionPresenter = new TransitionPresenter();
            this.transitionPresenter.Items.Add(this.oldContentPresenter);
            this.transitionPresenter.Items.Add(this.newContentPresenter);

            this.Children = new UIElementCollection(this, null);
            this.Children.Add(this.transitionPresenter);

            this.Loaded += (s, e) => this.loaded = true;
        }

        private static void OnTransitionChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NavigationPresenter)d).OnTransitionChanged(e);
        }

        private static void OnContentValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((NavigationPresenter)d).OnContentValueChanged(e);
        }

        private void OnContentValueChanged(DependencyPropertyChangedEventArgs e)
        {
            // switch ContentPresenters
            this.oldContentPresenter.Content = e.OldValue;
            this.newContentPresenter.Content = e.NewValue;

            if (this.loaded)
            {
                // if the transition supports the INavigationAware interface
                // prepare it using the GoForward/GoBackward methods
                if (this.transitionPresenter.Transition is INavigationAware)
                {
                    if (this.GoForward)
                        ((INavigationAware)this.transitionPresenter.Transition).GoForward();
                    else
                        ((INavigationAware)this.transitionPresenter.Transition).GoBackward();
                }

                // run the transition
                this.transitionPresenter.ApplyTransition(this.oldContentPresenter, this.newContentPresenter);
            }
        }

        private void OnTransitionChanged(DependencyPropertyChangedEventArgs e)
        {
            this.transitionPresenter.Transition = e.NewValue as Transition;
        }

        protected override Visual GetVisualChild(int index)
        {
            if ((index < 0) || (index >= this.Children.Count))
                throw new ArgumentOutOfRangeException("index");

            return this.Children[index];
        }


        protected override int VisualChildrenCount
        {
            get
            {
                if (this.Children != null)
                    return this.Children.Count;
                else
                    return 0;
            }
        }
    }
}
