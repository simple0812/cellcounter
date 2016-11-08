using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace EmguApp.libs
{
    /* ObjectForScriptingHelper helper = new ObjectForScriptingHelper(this);
            wb.ObjectForScripting = helper;
     */
    [System.Runtime.InteropServices.ComVisibleAttribute(true)]//将该类设置为com可访问 
    public class ObjectForScriptingHelper
    {
        Window mainWindow;

        public ObjectForScriptingHelper(Window main)
        {
            mainWindow = main;
        }


        public void Bar(string cmd)
        {
            Debug.WriteLine(cmd + "xxxxxxxxxxx");

        }

        public void Foo(IList<double> data )
        {
        }

    }
}
