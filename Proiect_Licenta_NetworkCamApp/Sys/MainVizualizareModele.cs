using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Proiect_Licenta_NetworkCamApp.View.SetariView;


namespace Proiect_Licenta_NetworkCamApp.Sys
{
    public class MainVizualizareModele
    {
        public MyViewModel MyViewModel { get; set; }
        public VizualizareSetari VizualizareSetari { get; set; }

        public MainVizualizareModele()
        {
            MyViewModel = new MyViewModel();
            VizualizareSetari = new VizualizareSetari();
        }
        /*
         * folosim legaturi multiple si le vom atribui unui singur obiect in MainWindows
        */

    }
}
