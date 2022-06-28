using OneOf;

namespace Domain;

public record ConnectionLost;
public record ResourceUnavailable;

public record NetworkError(OneOf<ConnectionLost, ResourceUnavailable> Wrapped);
