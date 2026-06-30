// Importa funciones básicas del sistema operativo, fechas y conversiones.
using System;
// Importa utilidades para la manipulación de flujos de bytes en memoria.
using System.IO;
// Importa métodos de extensión LINQ para búsquedas y mapeos rápidos en listas.
using System.Linq;
// Importa soporte para la programación de tareas asíncronas.
using System.Threading.Tasks;
// Importa la biblioteca de iText encargada de la escritura de ficheros PDF.
using iText.Kernel.Pdf;
// Importa la biblioteca de iText encargada de estructurar el diseño de página del PDF.
using iText.Layout;
// Importa elementos del documento de iText como párrafos, tablas y celdas.
using iText.Layout.Element;
// Importa enumeradores de alineación y estructuración espacial de iText.
using iText.Layout.Properties;
// Importa utilidades de configuración de color en el documento PDF.
using iText.Kernel.Colors;
// Importa los modelos definidos en el proyecto.
using TrafficFlow.Modelos;

namespace TrafficFlow.Servicios
{
    // Clase de servicio encargada de recopilar los datos y exportarlos en formato de archivo PDF.
    public class ServicioPDF
    {
        // Define la referencia al servicio de tránsito.
        private readonly ServicioTransito _servicioTransito;
        // Define la referencia al servicio de historial.
        private readonly ServicioHistorial _servicioHistorial;

        // Constructor que inyecta ambos servicios necesarios para consultar los datos del PDF.
        public ServicioPDF(ServicioTransito servicioTransito, ServicioHistorial servicioHistorial)
        {
            // Asigna el servicio de tránsito inyectado a la propiedad de clase.
            _servicioTransito = servicioTransito;
            // Asigna el servicio de historial inyectado a la propiedad de clase.
            _servicioHistorial = servicioHistorial;
        }

        // Genera y construye un reporte de tráfico completo en formato binario PDF.
        public async Task<byte[]> GenerarReporteTrafico()
        {
            // Obtiene de forma asíncrona la lista actual de rutas.
            var rutas = await _servicioTransito.ObtenerTodasLasRutas();
            // Obtiene de forma asíncrona el listado del historial guardado.
            var historial = await _servicioHistorial.ObtenerHistorial();

            // Crea un flujo de memoria intermedio para guardar los datos binarios del PDF.
            using var stream = new MemoryStream();
            // Inicializa el escritor de iText enlazado al flujo de memoria.
            using var escritor = new PdfWriter(stream);
            // Instancia el objeto de estructura del documento PDF.
            using var pdf = new PdfDocument(escritor);
            // Instancia la hoja de diseño para agregar elementos al documento.
            using var documento = new Document(pdf);

            // Agrega el párrafo del título principal del reporte con formato y colores específicos.
            documento.Add(new Paragraph("TrafficFlow — Reporte de Tráfico")
                .SetFontSize(22)
                .SimulateBold()
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontColor(new DeviceRgb(88, 166, 255)));

            // Agrega un párrafo secundario con la fecha y hora de la generación.
            documento.Add(new Paragraph($"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}")
                .SetFontSize(10)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontColor(new DeviceRgb(139, 148, 158)));

            // Inserta un salto de línea para espaciar los elementos.
            documento.Add(new Paragraph("\n"));


            // Agrega el título de la sección de resumen general.
            documento.Add(new Paragraph("Resumen General")
                .SetFontSize(14)
                .SimulateBold()
                .SetFontColor(new DeviceRgb(88, 166, 255)));

            // Crea una tabla de resumen con dos columnas adaptada al ancho disponible de página.
            var tabla = new Table(2).UseAllAvailableWidth();
            // Añade la celda de encabezado de la primera columna.
            tabla.AddCell(CeldaEncabezado("Indicador"));
            // Añade la celda de encabezado de la segunda columna.
            tabla.AddCell(CeldaEncabezado("Valor"));
            // Inserta la fila indicando el total de rutas registradas.
            tabla.AddCell(new Cell().Add(new Paragraph("Total de rutas activas")));
            // Muestra el valor numérico del total de rutas.
            tabla.AddCell(new Cell().Add(new Paragraph(rutas.Count.ToString())));
            // Inserta la fila correspondiente al recuento de congestión alta.
            tabla.AddCell(new Cell().Add(new Paragraph("Rutas con tráfico alto")));
            // Calcula y muestra el total de rutas con nivel de congestión alto.
            tabla.AddCell(new Cell().Add(new Paragraph(rutas.Count(r => r.Segmentos.Any(s => s.NivelTransito == "alto")).ToString())));
            // Inserta la fila correspondiente al recuento de congestión media.
            tabla.AddCell(new Cell().Add(new Paragraph("Rutas con tráfico medio")));
            // Calcula y muestra el total de rutas con nivel de congestión medio.
            tabla.AddCell(new Cell().Add(new Paragraph(rutas.Count(r => r.Segmentos.Any(s => s.NivelTransito == "medio")).ToString())));
            // Inserta la fila correspondiente al recuento de congestión baja.
            tabla.AddCell(new Cell().Add(new Paragraph("Rutas con tráfico bajo")));
            // Calcula y muestra el total de rutas con nivel de congestión bajo.
            tabla.AddCell(new Cell().Add(new Paragraph(rutas.Count(r => r.Segmentos.Any(s => s.NivelTransito == "bajo")).ToString())));
            // Inserta la fila del total de mediciones del historial guardadas.
            tabla.AddCell(new Cell().Add(new Paragraph("Total registros historial")));
            // Muestra el conteo de elementos del historial.
            tabla.AddCell(new Cell().Add(new Paragraph(historial.Count.ToString())));

            // Agrega la tabla de resumen al documento PDF.
            documento.Add(tabla);
            // Inserta un espaciador de línea.
            documento.Add(new Paragraph("\n"));

            // Agrega el título de la sección de detalle de rutas.
            documento.Add(new Paragraph("Detalle de Rutas Activas")
                .SetFontSize(14)
                .SimulateBold()
                .SetFontColor(new DeviceRgb(88, 166, 255)));

            // Crea una tabla de cuatro columnas para mostrar el detalle de cada ruta.
            var tablaRutas = new Table(4).UseAllAvailableWidth();
            // Agrega el encabezado para el nombre de la ruta.
            tablaRutas.AddCell(CeldaEncabezado("Ruta"));
            // Agrega el encabezado para el origen.
            tablaRutas.AddCell(CeldaEncabezado("Origen"));
            // Agrega el encabezado para el destino.
            tablaRutas.AddCell(CeldaEncabezado("Destino"));
            // Agrega el encabezado para el nivel del tráfico.
            tablaRutas.AddCell(CeldaEncabezado("Nivel tráfico"));

            // Recorre y agrega la información específica de cada una de las rutas.
            foreach (var ruta in rutas)
            {
                // Agrega el nombre de la ruta a la tabla.
                tablaRutas.AddCell(new Cell().Add(new Paragraph(ruta.Nombre)));
                // Agrega el origen geográfico a la tabla.
                tablaRutas.AddCell(new Cell().Add(new Paragraph(ruta.Origen)));
                // Agrega el destino geográfico a la tabla.
                tablaRutas.AddCell(new Cell().Add(new Paragraph(ruta.Destino)));
                // Agrega el estado de tráfico del tramo inicial.
                tablaRutas.AddCell(new Cell().Add(new Paragraph(ruta.Segmentos.FirstOrDefault()?.NivelTransito ?? "N/A")));
            }

            // Agrega la tabla detallada de rutas al documento PDF.
            documento.Add(tablaRutas);
            // Inserta un espaciador de línea.
            documento.Add(new Paragraph("\n"));

            // Valida si existen registros para imprimir la sección de historial.
            if (historial.Any())
            {
                // Agrega el encabezado de la sección del historial de mediciones.
                documento.Add(new Paragraph("Historial Reciente (últimos 10 registros)")
                    .SetFontSize(14)
                    .SimulateBold()
                    .SetFontColor(new DeviceRgb(88, 166, 255)));

                // Crea una tabla de cuatro columnas para desglosar el historial reciente.
                var tablaHistorial = new Table(4).UseAllAvailableWidth();
                // Añade el encabezado de ruta.
                tablaHistorial.AddCell(CeldaEncabezado("Ruta"));
                // Añade el encabezado de nivel de tráfico.
                tablaHistorial.AddCell(CeldaEncabezado("Nivel"));
                // Añade el encabezado de velocidad.
                tablaHistorial.AddCell(CeldaEncabezado("Velocidad"));
                // Añade el encabezado de fecha.
                tablaHistorial.AddCell(CeldaEncabezado("Fecha"));

                // Recorre y agrega los 10 registros históricos más recientes de la base de datos.
                foreach (var registro in historial.Take(10))
                {
                    // Inserta el nombre de la ruta asociada.
                    tablaHistorial.AddCell(new Cell().Add(new Paragraph(registro.NombreRuta)));
                    // Inserta la congestión vial registrada.
                    tablaHistorial.AddCell(new Cell().Add(new Paragraph(registro.NivelTransito)));
                    // Inserta la velocidad promedio calculada.
                    tablaHistorial.AddCell(new Cell().Add(new Paragraph($"{registro.VelocidadPromedio} km/h")));
                    // Inserta la fecha local convertida del registro.
                    tablaHistorial.AddCell(new Cell().Add(new Paragraph(registro.FechaRegistro.ToLocalTime().ToString("dd/MM/yyyy HH:mm"))));
                }

                // Agrega la tabla histórica al documento PDF.
                documento.Add(tablaHistorial);
            }

            // Cierra la edición del documento y libera los recursos.
            documento.Close();
            // Retorna los bytes acumulados en memoria correspondientes al archivo PDF completo.
            return stream.ToArray();
        }

        // Construye y estiliza una celda estándar para ser utilizada como encabezado de tabla.
        private Cell CeldaEncabezado(string texto)
        {
            // Retorna una celda con fondo oscuro, fuente de color e indicador en negrita.
            return new Cell().Add(new Paragraph(texto).SimulateBold())
                .SetBackgroundColor(new DeviceRgb(22, 27, 34))
                .SetFontColor(new DeviceRgb(88, 166, 255));
        }
    }
}