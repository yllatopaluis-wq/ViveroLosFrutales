using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace ViveroLosFrutales.Web.Controllers;

public static class ErrorMessageHelper
{
    public static string ToSpanish(Exception exception)
    {
        if (exception is DbUpdateException dbUpdateException)
        {
            if (dbUpdateException.InnerException is SqlException sqlException)
            {
                return sqlException.Number switch
                {
                    207 => "La base de datos no esta actualizada o falta una columna requerida. Ejecute los scripts de actualizacion y vuelva a intentar.",
                    2601 or 2627 => "Ya existe un registro con los mismos datos. Revise que el documento no este duplicado.",
                    547 => "No se puede guardar o eliminar el registro porque tiene informacion relacionada.",
                    8152 or 2628 => "Uno de los textos ingresados supera el largo permitido.",
                    _ => $"No se pudo guardar la informacion. Detalle tecnico SQL {sqlException.Number}."
                };
            }

            return "No se pudo guardar la informacion. Revise los datos ingresados e intente nuevamente.";
        }

        return exception switch
        {
            InvalidOperationException => exception.Message,
            ArgumentException => exception.Message,
            _ => string.IsNullOrWhiteSpace(exception.Message)
                ? "No se pudo completar la solicitud."
                : TraducirMensajeComun(exception.Message)
        };
    }

    private static string TraducirMensajeComun(string mensaje)
    {
        if (mensaje.Contains("Object reference not set to an instance of an object", StringComparison.OrdinalIgnoreCase))
        {
            return "No se pudo completar la solicitud porque falta informacion requerida.";
        }

        if (mensaje.Contains("An error occurred while saving the entity changes", StringComparison.OrdinalIgnoreCase))
        {
            return "No se pudo guardar la informacion. Revise los datos ingresados e intente nuevamente.";
        }

        if (mensaje.Contains("The field", StringComparison.OrdinalIgnoreCase) && mensaje.Contains("is required", StringComparison.OrdinalIgnoreCase))
        {
            return "Complete los campos obligatorios.";
        }

        return mensaje;
    }
}
