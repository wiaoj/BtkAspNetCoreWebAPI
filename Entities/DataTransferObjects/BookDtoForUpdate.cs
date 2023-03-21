using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects;
public record BookDtoForUpdate([Required] Guid Id) : BookDtoForManipulation;