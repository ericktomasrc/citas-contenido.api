using FluentValidation.Results;
using System.Xml.Serialization;
namespace Smr.Backend.Shared
{
    public class StatusResponse
    {
        public Guid Id { get; set; }

        private bool _success;
        public bool success
        {
            get { return this._success; }
            set
            {
                this._success = value;
            }
        }


        public string title { get; set; }


        private int _statusCode = 200;
        public int statusCode
        {
            get
            {
                if (!this._success && this._statusCode == 200)
                    return 500;

                return this._statusCode;
            }
            set
            {
                this._statusCode = value;
            }
        }

        public string? exception { get; set; }
        public string? detail { get; set; }
        public List<ValidacionMasiva>? validacionesMasivas { get; set; }

        [XmlIgnore]
        public Dictionary<string, List<string>>? errors { get; set; }

        public StatusResponse() : this(true) { }
        public StatusResponse(bool isSuccess) : this(isSuccess, "") { }
        public StatusResponse(bool isSuccess, string msgTitle) : this(isSuccess, msgTitle, null) { }
        public StatusResponse(bool isValid, string title, IList<ValidationFailure> listOfErrors)
           : this(isValid, title, null  , listOfErrors)
        {
        }

        public StatusResponse(bool isValid, string title, string detail, IList<ValidationFailure> listOfErrors)
        {
            this.Id = Guid.NewGuid();
            this.success = isValid;
            this.title = title;
            this.detail = detail;


            if (listOfErrors != null)
            {
                Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
                foreach (ValidationFailure item in listOfErrors)
                {
                    List<string>? errorsFromProperties = null;
                    if (!_errors.TryGetValue(item.PropertyName, out errorsFromProperties))
                    {
                        errorsFromProperties = new List<string>();
                        _errors.Add(item.PropertyName, errorsFromProperties);
                    }

                    errorsFromProperties.Add(item.ErrorMessage);
                }
                this.errors = _errors;
            }
        }
    }


    public class StatusResponse<T> : StatusResponse
    {

        public T? data { get; set; }

        public StatusResponse() : this(true, "") { }

        public StatusResponse(bool success, string title)
        {
            base.success = success;
            base.title = title;
        }

        public StatusResponse(bool isValid, string title, IList<ValidationFailure> listOfErrors)
           : this(isValid, title, null, listOfErrors)
        {
        }

        public StatusResponse(bool isValid, string title, string detail, IList<ValidationFailure> listOfErrors)
        {
            base.Id = Guid.NewGuid();
            base.success = isValid;
            base.title = title;
            base.detail = detail;

            if (listOfErrors != null)
            {
                Dictionary<string, List<string>> _errors = new Dictionary<string, List<string>>();
                foreach (ValidationFailure item in listOfErrors)
                {
                    List<string>? errorsFromProperties = null;
                    if (!_errors.TryGetValue(item.PropertyName, out errorsFromProperties))
                    {
                        errorsFromProperties = new List<string>();
                        _errors.Add(item.PropertyName, errorsFromProperties);
                    }

                    errorsFromProperties.Add(item.ErrorMessage);
                }
                base.errors = _errors;
            }
        }
    }

    public class ValidacionMasiva
    {
        public int Fila { get; set; }
        public string? Serie { get; set; }
        public string? Modelo { get; set; }
        public string? Mensaje { get; set; }
    }
}
