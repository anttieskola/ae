using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace AE.WebUI.Filters
{
    public class MinificationStream : Stream
    {
        private Stream _s;
        private Encoding _e;

        public MinificationStream(Stream s, Encoding e)
        {
            _s = s;
            _e = e;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (_e == Encoding.UTF8
                && offset == 0 && count > 3)
            {
                byte[] b = new byte[count];
                Array.Copy(buffer, b, count);
                string n = Encoding.UTF8.GetString(b);

                int oL = n.Length;

                // breaks javascript from sections :/
                if (!n.Contains("script>"))
                {
                    Regex pat;
                    pat = new Regex(@"[\n\t]*"); // line feeds and tabs
                    n = pat.Replace(n, "");

                    pat = new Regex(@">\s+"); // spaces between elements or elements and content
                    n = pat.Replace(n, ">");

                    pat = new Regex(@"\s+"); // extra spaces just one is enough
                    n = pat.Replace(n, " ");
                }

                int nL = n.Length;

                System.Diagnostics.Debug.WriteLine("{0} => {1}", oL, nL);

                byte[] nB = Encoding.UTF8.GetBytes(n);
                _s.Write(nB, offset, nL);
            }
            else
            {
                _s.Write(buffer, offset, count);
            }
        }
        public override void SetLength(long value)
        {
            _s.SetLength(value);
        }
        public override long Seek(long offset, SeekOrigin origin)
        {
            return _s.Seek(offset, origin);
        }
        public override int Read(byte[] buffer, int offset, int count)
        {
            return _s.Read(buffer, offset, count);
        }
        public override long Position
        {
            get
            {
                return _s.Position;
            }
            set
            {
                _s.Position = value;
            }
        }
        public override long Length
        {
            get
            {
                return _s.Length;
            }
        }
        public override void Flush()
        {
            _s.Flush();
        }
        public override bool CanWrite
        {
            get
            {
                return _s.CanWrite;
            }
        }
        public override bool CanSeek
        {
            get
            {
                return _s.CanSeek;
            }

        }
        public override bool CanRead
        {
            get
            {
                return _s.CanRead;
            }
        }
    }

}