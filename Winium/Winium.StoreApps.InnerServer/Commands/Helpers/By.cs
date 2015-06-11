﻿namespace Winium.StoreApps.InnerServer.Commands.Helpers
{
    #region

    using System;

    using Windows.UI.Xaml;

    #endregion

    internal class By
    {
        #region Constructors and Destructors

        public By(string strategy, string value)
        {
            if (strategy.Equals("tag name") || strategy.Equals("class name"))
            {
                this.Predicate = x => x.GetType().ToString().Equals(value);
            }
            else if (strategy.Equals("id"))
            {
                this.Predicate = x =>
                    {
                        FrameworkElement element = (x as FrameworkElement);
                        if (element == null)
                            return false;

                        return element.Name.Equals(value);
                    };
            }
            else if (strategy.Equals("name"))
            {

                this.Predicate = x =>
                {
                    var automationId = "";

                    FrameworkElement element = x as FrameworkElement;
                    if (element != null)
                        automationId = element.Name;

                    return automationId.Equals(value);
                };
            }
            else if (strategy.Equals("xname"))
            {
                // TODO: transitional. to be depricated
                this.Predicate = x =>
                    {
                        var frameworkElement = x as FrameworkElement;
                        return frameworkElement != null && frameworkElement.Name.Equals(value);
                    };
            }
            else
            {
                throw new NotImplementedException(
                    string.Format("{0} is not valid or implemented searching strategy.", strategy));
            }
        }

        #endregion

        #region Public Properties

        public Predicate<DependencyObject> Predicate { get; private set; }

        #endregion
    }
}
