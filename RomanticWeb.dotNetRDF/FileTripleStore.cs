using System;
using System.IO;
using System.Linq;
using VDS.RDF;
using VDS.RDF.Parsing;
using VDS.RDF.Query.Datasets;
using VDS.RDF.Update;
using VDS.RDF.Writing;

namespace RomanticWeb.DotNetRDF
{
    /// <summary>Provides a basic implementation of a file based updateable triple store.</summary>
    public class FileTripleStore : TripleStore, IUpdateableTripleStore
    {
        private const int MaxTries = 5;
        private static readonly object Locker = new Object();

        private string _filePath = null;
        private Stream _fileStream = null;
        private IRdfReader _rdfReader = null;
        private IRdfWriter _rdfWriter = null;
        private IStoreReader _storeReader = null;
        private IStoreWriter _storeWriter = null;

        /// <summary>Creates a new instance of the file triple store.</summary>
        /// <param name="filePath">Path of the file to read/write.</param>
        public FileTripleStore(string filePath)
        {
            CreateIOHandlers(Path.GetExtension(filePath).ToLower());
            if (!File.Exists(_filePath = filePath))
            {
                File.Create(filePath).Close();
            }

            Read();
        }

        /// <summary>Creates a new instance of the file triple store.</summary>
        /// <param name="filePath">Path of the file to read/write.</param>
        /// <param name="storeReader">Store reader used to read the file.</param>
        /// <param name="storeWriter">Store writer to write the file.</param>
        public FileTripleStore(string filePath, IStoreReader storeReader, IStoreWriter storeWriter)
        {
            if (!File.Exists(_filePath = filePath))
            {
                File.Create(filePath).Close();
            }

            _storeReader = storeReader;
            _storeWriter = storeWriter;

            Read();
        }

        /// <summary>Creates a new instance of the file triple store.</summary>
        /// <param name="filePath">Path of the file to read/write.</param>
        /// <param name="rdfReader">RDF reader used to read the file.</param>
        /// <param name="rdfWriter">RDF writer to write the file.</param>
        public FileTripleStore(string filePath, IRdfReader rdfReader, IRdfWriter rdfWriter)
        {
            if (!File.Exists(_filePath = filePath))
            {
                File.Create(filePath).Close();
            }

            _rdfReader = rdfReader;
            _rdfWriter = rdfWriter;

            Read();
        }

        /// <summary>Creates a new instance of the file triple store.</summary>
        /// <param name="fileStream">Stream to read/write.</param>
        /// <param name="storeReader">Store reader used to read the file.</param>
        /// <param name="storeWriter">Store writer to write the file.</param>
        public FileTripleStore(Stream fileStream, IStoreReader storeReader, IStoreWriter storeWriter)
        {
            _fileStream = fileStream;
            _storeReader = storeReader;
            _storeWriter = storeWriter;
            Read();
        }

        /// <summary>Creates a new instance of the file triple store.</summary>
        /// <param name="fileStream">Stream to read/write.</param>
        /// <param name="rdfReader">RDF reader used to read the file.</param>
        /// <param name="rdfWriter">RDF writer to write the file.</param>
        public FileTripleStore(Stream fileStream, IRdfReader rdfReader, IRdfWriter rdfWriter)
        {
            _fileStream = fileStream;
            _rdfReader = rdfReader;
            _rdfWriter = rdfWriter;
            Read();
        }

        /// <inheritdoc />
        public new void ExecuteUpdate(SparqlUpdateCommandSet updates)
        {
            ((IUpdateableTripleStore)this).ExecuteUpdate(updates);
        }

        /// <inheritdoc />
        public new void ExecuteUpdate(SparqlUpdateCommand update)
        {
            ((IUpdateableTripleStore)this).ExecuteUpdate(update);
        }

        /// <inheritdoc />
        public new void ExecuteUpdate(string update)
        {
            ((IUpdateableTripleStore)this).ExecuteUpdate(update);
        }

        /// <inheritdoc />
        void IUpdateableTripleStore.ExecuteUpdate(SparqlUpdateCommandSet updates)
        {
            lock (Locker)
            {
                LeviathanUpdateProcessor processor = new LeviathanUpdateProcessor(this);
                processor.ProcessCommandSet(updates);
                Write();
            }
        }

        /// <inheritdoc />
        void IUpdateableTripleStore.ExecuteUpdate(SparqlUpdateCommand update)
        {
            ((IUpdateableTripleStore)this).ExecuteUpdate(new SparqlUpdateCommandSet(update));
        }

        /// <inheritdoc />
        void IUpdateableTripleStore.ExecuteUpdate(string update)
        {
            ((IUpdateableTripleStore)this).ExecuteUpdate(new SparqlUpdateCommandSet(new SparqlUpdateParser().ParseFromString(update).Commands));
        }

        private void CreateIOHandlers(string extension)
        {
            switch (extension)
            {
                case ".nq":
                    _storeReader = new NQuadsParser();
                    _storeWriter = new NQuadsWriter();
                    break;
                case ".ttl":
                    _rdfReader = new TurtleParser();
                    _rdfWriter = new CompressingTurtleWriter();
                    break;
                case ".trig":
                    _storeReader = new TriGParser();
                    _storeWriter = new TriGWriter();
                    break;
                case ".xml":
                    _rdfReader = new RdfXmlParser();
                    _rdfWriter = new RdfXmlWriter();
                    break;
                case ".n3":
                    _rdfReader = new Notation3Parser();
                    _rdfWriter = new Notation3Writer();
                    break;
                case ".trix":
                    _storeReader = new TriXParser();
                    _storeWriter = new TriXWriter();
                    break;
                case ".json":
                    _rdfReader = new RdfJsonParser();
                    _rdfWriter = new RdfJsonWriter();
                    break;
                default:
                    throw new ArgumentOutOfRangeException(System.String.Format("Provided file path does not allow to detect a type of the RDF serialization type."));
            }
        }

        private void Read()
        {
            lock (Locker)
            {
                if (_storeReader != null)
                {
                    ReadStore();
                }
                else
                {
                    ReadGraphs();
                }
            }
        }

        private void ReadStore()
        {
            if (_filePath != null)
            {
                _storeReader.Load(this, _filePath);
            }
            else
            {
                _fileStream.Seek(0, SeekOrigin.Begin);
                TextReader fileReader = new StreamReader(_fileStream, System.Text.UTF8Encoding.UTF8, true, 4096, true);
                _storeReader.Load(this, fileReader);
            }
        }

        private void ReadGraphs()
        {
            IGraph graph = this.Graphs.FirstOrDefault();
            if (graph == null)
            {
                graph = new Graph();
                this.Add(graph);
            }

            if (_filePath != null)
            {
                _rdfReader.Load(graph, _filePath);
            }
            else
            {
                _fileStream.Seek(0, SeekOrigin.Begin);
                TextReader fileReader = new StreamReader(_fileStream, System.Text.UTF8Encoding.UTF8, true, 4096, true);
                _rdfReader.Load(graph, fileReader);
            }
        }

        private void Write()
        {
            int tries = 0;
            while (tries < MaxTries)
            {
                try
                {
                    if (_storeWriter != null)
                    {
                        WriteStore();
                    }
                    else
                    {
                        WriteGraphs();
                    }

                    tries = MaxTries;
                }
                catch (IOException)
                {
                    tries++;
                    System.Threading.Thread.Sleep(100);
                }
            }
        }

        private void WriteStore()
        {
            if (_filePath != null)
            {
                _storeWriter.Save(this, _filePath);
            }
            else
            {
                _fileStream.SetLength(0);
                TextWriter fileWriter = new StreamWriter(_fileStream, System.Text.UTF8Encoding.UTF8, 4096, true);
                _storeWriter.Save(this, fileWriter);
                _fileStream.Flush();
            }
        }

        private void WriteGraphs()
        {
            Stream fileStream = _fileStream;
            TextWriter fileWriter = null;
            try
            {
                if (fileStream == null)
                {
                    fileStream = File.Open(_filePath, FileMode.Open, FileAccess.Write);
                }
                else
                {
                    _fileStream.SetLength(0);
                }

                fileWriter = new StreamWriter(fileStream);
                foreach (IGraph graph in this.Graphs)
                {
                    _rdfWriter.Save(graph, fileWriter);
                }

                fileStream.Flush();
            }
            finally
            {
                if (_fileStream == null)
                {
                    if (fileWriter != null)
                    {
                        fileWriter.Close();
                    }

                    if (fileStream != null)
                    {
                        fileStream.Close();
                    }
                }
            }
        }
    }
}