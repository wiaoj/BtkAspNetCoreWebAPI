using System.ComponentModel.DataAnnotations;

namespace Entities.DataTransferObjects;
public record BookDtoForInsertion([Required(ErrorMessage = "CategoryId is required.")] Guid CategoryId) : BookDtoForManipulation;