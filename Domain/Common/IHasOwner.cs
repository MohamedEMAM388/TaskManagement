namespace Domain.Common;

public interface IHasOwner
{
    public string CreatedByUserId { get; }
}