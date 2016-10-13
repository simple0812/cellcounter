using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace EmguApp.libs
{
    public static class DependencyObjectExtensions
    {
        public static T FindChild<T>(this DependencyObject root) where T : UIElement
        {
            var queue = new Queue<DependencyObject>();
            queue.Enqueue(root);
            while (queue.Count > 0)
            {
                var dependencyObject = queue.Dequeue();
                var num = VisualTreeHelper.GetChildrenCount(dependencyObject) - 1;
                while (0 <= num)
                {
                    var child = VisualTreeHelper.GetChild(dependencyObject, num);
                    var t = child as T;
                    if (t != null)
                    {
                        return t;
                    }
                    queue.Enqueue(child);
                    num--;
                }
            }
            return default(T);
        }

        public static T FindByName<T>(this DependencyObject obj, string name)
            where T : UIElement
        {
            if (obj.GetType() == typeof(T) && (obj as FrameworkElement)?.Name == name)
            {
                return obj as T;
            }

            var childrenCount = VisualTreeHelper.GetChildrenCount(obj);

            if (childrenCount == 0)
            {
                return default(T);
            }

            for (var i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(obj, i);
                var t = FindByName<T>(child, name);

                if (t != null)
                {
                    return t;
                }
            }

            return default(T);
        }

        public static List<T> FindChildren<T>(this DependencyObject parent) where T : UIElement
        {
            var visualCollection = new List<T>();
            GetVisualChildCollection(parent, visualCollection);
            return visualCollection;
        }

        private static void GetVisualChildCollection<T>(this DependencyObject parent, List<T> visualCollection) where T : UIElement
        {
            var count = VisualTreeHelper.GetChildrenCount(parent);
            for (var i = 0; i < count; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                if (child is T)
                {
                    visualCollection.Add(child as T);
                }
                else if (child != null)
                {
                    GetVisualChildCollection(child, visualCollection);
                }
            }
        }
    }
}
