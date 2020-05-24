using System.IO;

namespace mashovFinal.Utilities
{
    public class NpoiMemoryStream : MemoryStream
    {
        // We always want to close streams by default to
        // force the developer to make the conscious decision
        // to disable it.  Then, they're more apt to remember
        // to re-enable it.  The last thing you want is to
        // enable memory leaks by default.  ;-)

        public bool AllowClose { get; set; } = true;

        public override void Close()
        {
            if (AllowClose)
                base.Close();
        }
    }
}