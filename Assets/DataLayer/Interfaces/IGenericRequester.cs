using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Assets.DataLayer.Interfaces
{
    public interface IGenericRequester
    {
        string Endpoint { get; }


        IEnumerator GetObject<Tout>(string path, Action<Tout> responseHandler = null);

        IEnumerator PostObject<Tin, Tout>(Tin bodyObj, string path, Action<Tout> responseHandler = null);

        IEnumerator PostObjectSections<Tout>(Dictionary<string, (object, string)> formData, string path, Action<Tout> responseHandler = null);

    }
}
