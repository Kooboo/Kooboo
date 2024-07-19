namespace Kooboo.Web.ViewModel
{
    public class DNSRecordViewModel
    {
        public Guid Id { get; set; }
        public string Domain { get; set; }
        public string Host { get; set; }
        public string Type { get; set; }
        public string Value { get; set; }
        public int Priority { get; set; }
        public int TTL { get; set; }
    }
}


/*

private Guid _id;
public Guid Id
{
    get
    {
        if (_id == default(Guid))
        {
            string unique = this.Domain + this.Host + this.Priority.ToString() + this.Type;
            _id = Lib.Security.Hash.ComputeGuidIgnoreCase(unique);
        }
        return _id;
    }
    set { _id = value; }
}

private Guid _domainid;
public Guid DomainId
{
    get
    {
        if (_domainid == default(Guid))
        {
            if (!string.IsNullOrWhiteSpace(this.Domain))
            {
                _domainid = Lib.Security.Hash.ComputeGuidIgnoreCase(this.Domain);
            }
        }
        return _domainid;
    }
    set
    {
        _domainid = value;
    }
}

public string Host { get; set; }

public string Value { get; set; }

public string Type { get; set; } = "A";

public string Domain { get; set; }

public int Priority { get; set; }

*/