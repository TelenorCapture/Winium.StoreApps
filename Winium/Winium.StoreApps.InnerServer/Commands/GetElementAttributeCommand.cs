using System.Collections.Generic;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;

namespace Winium.StoreApps.InnerServer.Commands
{
    using System;
    using System.Reflection;

    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    using Winium.StoreApps.Common;
    using Winium.StoreApps.Common.Exceptions;
    using Winium.StoreApps.InnerServer.Commands.Helpers;
    using Windows.UI.Xaml.Controls;

    internal class GetElementAttributeCommand : CommandBase
    {
        public string ElementId { get; set; }

        public override string DoImpl()
        {
            var element = this.Automator.WebElements.GetRegisteredElement(this.ElementId);

            JToken value;
            string attributeName = null;
            if (this.Parameters.TryGetValue("NAME", out value))
            {
                attributeName = value.ToString();
            }

            if (attributeName == null)
            {
                return this.JsonResponse();
            }

            /* GetAttribute command should return: null if no property was found,
             * property value as plain string if property is scalar or string,
             * JSON encoded property if property is Lists, Dictionary or other nonscalar types 
             */
            try
            {
                // This is a special case were we're trying to find all the elements that are visibile in Timeline
                if (attributeName == "TL_Children" && element is ListView) {
                    return JsonResponse(ResponseStatus.Success, SerializeObjectAsString(GetHashList(element)));
                } 
                if (attributeName == "TL_ListView_Pos" && element is ListView) {
                    return JsonResponse(ResponseStatus.Success, SerializeObjectAsString(GetEstimatedListPos(element)));
                }
                else
                {
                    var propertyObject = element.GetAttribute(attributeName);
                    return this.JsonResponse(ResponseStatus.Success, propertyObject);
                }
            }
            catch (AutomationException e)
            {
                return this.JsonResponse();
            }
        }

        private List<int> GetHashList(FrameworkElement element)
        {
            ListView lv = element as ListView;
            List<int> hashList = new List<int>();

            foreach (var ch in lv.ItemsPanelRoot.Children)
            {
                ListViewItem lvi = ch as ListViewItem;
                if (lvi == null)
                    continue;

                if (!IsInBounds(lvi, lv))
                    continue;

                // TODO: What should we return here?
                if (lvi.Content != null)
                    hashList.Add(lvi.Content.GetHashCode());

                /*
                IPhotoViewModel pvm = lvi.Content as IPhotoViewModel;

                if (pvm != null)
                {
                    hashList.Add(pvm.GetHashCode());
                }
                */
            }
            return hashList;
        }

        private double GetEstimatedListPos(FrameworkElement element)
        {
            ListView listView = element as ListView;

            if (listView?.ItemsPanelRoot?.Children == null)
                return 0.0f;

            if (listView.ItemsPanelRoot.Children.Count == 0)
                throw new Exception("List View is empty");

            UIElementCollection collection = listView.ItemsPanelRoot.Children;

            double total = 0.0;
            int count = 0;

            foreach (var item in collection) {
                ListViewItem lvi = item as ListViewItem;

                if (lvi == null )
                    continue;

                GeneralTransform gt = item.TransformToVisual(listView);
                Point offset = gt.TransformPoint(new Point(0, 0));

                total += offset.Y;
                ++count;
            }

            if (count == 0)
                return 0;

            return ( total / count );
        }

        private bool IsInBounds(FrameworkElement element, ListView lv)
        {
            Point point;
            point.X = 0;
            point.Y = 0;

            GeneralTransform gt = element.TransformToVisual(lv);
            Point offset = gt.TransformPoint(point);

            bool xResult = offset.X + element.ActualWidth >= 0 && offset.X < lv.Width;
            bool yResult = offset.Y + element.ActualHeight >= 0 && offset.Y < lv.ActualHeight;

            return xResult && yResult;
        }

        private static bool IsTypeSerializedUsingToString(Type type)
        {
            // Strings should be serialized as plain strings
            return type == typeof(string) || type.GetTypeInfo().IsPrimitive;
        }

        private static string SerializeObjectAsString(object obj)
        {
            if (obj == null)
            {
                return null;
            }

            // Serialize basic types as palin strings
            if (IsTypeSerializedUsingToString(obj.GetType()))
            {
                return obj.ToString();
            }

            // Serialize other data types as JSON
            return JsonConvert.SerializeObject(obj);
        }
    }
}
