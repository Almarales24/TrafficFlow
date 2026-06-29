using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using iText.Kernel.Colors;
using TrafficFlow.Modelos;

namespace TrafficFlow.Servicios
{
    public class ServicioPDF
    {
        private readonly ServicioTransito _servicioTransito;
        private readonly ServicioHistorial _servicioHistorial;

        public ServicioPDF(ServicioTransito servicioTransito, ServicioHistorial servicioHistorial)
        {
            _servicioTransito = servicioTransito;
            _servicioHistorial = servicioHistorial;
        }

        public async Task<byte[]> GenerarReporteTrafico()
        {
            var rutas = await _servicioTransito.ObtenerTodasLasRutas();
            var historial = await _servicioHistorial.ObtenerHistorial();

            using var stream = new MemoryStream();
            using var escritor = new PdfWriter(stream);
            using var pdf = new PdfDocument(escritor);
            using var documento = new Document(pdf);

            // Título
            documento.Add(new Paragraph("TrafficFlow — Reporte de Tráfico")
                .SetFontSize(22)
                .SimulateBold()
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontColor(new DeviceRgb(88, 166, 255)));

            documento.Add(new Paragraph($"Generado el: {DateTime.Now:dd/MM/yyyy HH:mm}")
                .SetFontSize(10)
                .SetTextAlignment(TextAlignment.CENTER)
                .SetFontColor(new DeviceRgb(139, 148, 158)));

            documento.Add(new Paragraph("\n"));


            // Resumen general
            documento.Add(new Paragraph("Resumen General")
                .SetFontSize(14)
                .SimulateBold()
                .SetFontColor(new DeviceRgb(88, 166, 255)));

            var tabla = new Table(2).UseAllAvailableWidth();
            tabla.AddCell(CeldaEncabezado("Indicador"));
            tabla.AddCell(CeldaEncabezado("Valor"));
            tabla.AddCell(new Cell().Add(new Paragraph("Total de rutas activas")));
            tabla.AddCell(new Cell().Add(new Paragraph(rutas.Count.ToString())));
            tabla.AddCell(new Cell().Add(new Paragraph("Rutas con tráfico alto")));
            tabla.AddCell(new Cell().Add(new Paragraph(rutas.Count(r => r.Segmentos.Any(s => s.NivelTransito == "alto")).ToString())));
            tabla.AddCell(new Cell().Add(new Paragraph("Rutas con tráfico medio")));
            tabla.AddCell(new Cell().Add(new Paragraph(rutas.Count(r => r.Segmentos.Any(s => s.NivelTransito == "medio")).ToString())));
            tabla.AddCell(new Cell().Add(new Paragraph("Rutas con tráfico bajo")));
            tabla.AddCell(new Cell().Add(new Paragraph(rutas.Count(r => r.Segmentos.Any(s => s.NivelTransito == "bajo")).ToString())));
            tabla.AddCell(new Cell().Add(new Paragraph("Total registros historial")));
            tabla.AddCell(new Cell().Add(new Paragraph(historial.Count.ToString())));

            documento.Add(tabla);
            documento.Add(new Paragraph("\n"));

            // Detalle de rutas
            documento.Add(new Paragraph("Detalle de Rutas Activas")
                .SetFontSize(14)
                .SimulateBold()
                .SetFontColor(new DeviceRgb(88, 166, 255)));

            var tablaRutas = new Table(4).UseAllAvailableWidth();
            tablaRutas.AddCell(CeldaEncabezado("Ruta"));
            tablaRutas.AddCell(CeldaEncabezado("Origen"));
            tablaRutas.AddCell(CeldaEncabezado("Destino"));
            tablaRutas.AddCell(CeldaEncabezado("Nivel tráfico"));

            foreach (var ruta in rutas)
            {
                tablaRutas.AddCell(new Cell().Add(new Paragraph(ruta.Nombre)));
                tablaRutas.AddCell(new Cell().Add(new Paragraph(ruta.Origen)));
                tablaRutas.AddCell(new Cell().Add(new Paragraph(ruta.Destino)));
                tablaRutas.AddCell(new Cell().Add(new Paragraph(ruta.Segmentos.FirstOrDefault()?.NivelTransito ?? "N/A")));
            }

            documento.Add(tablaRutas);
            documento.Add(new Paragraph("\n"));

            // Historial reciente
            if (historial.Any())
            {
                documento.Add(new Paragraph("Historial Reciente (últimos 10 registros)")
                    .SetFontSize(14)
                    .SimulateBold()
                    .SetFontColor(new DeviceRgb(88, 166, 255)));

                var tablaHistorial = new Table(4).UseAllAvailableWidth();
                tablaHistorial.AddCell(CeldaEncabezado("Ruta"));
                tablaHistorial.AddCell(CeldaEncabezado("Nivel"));
                tablaHistorial.AddCell(CeldaEncabezado("Velocidad"));
                tablaHistorial.AddCell(CeldaEncabezado("Fecha"));

                foreach (var registro in historial.Take(10))
                {
                    tablaHistorial.AddCell(new Cell().Add(new Paragraph(registro.NombreRuta)));
                    tablaHistorial.AddCell(new Cell().Add(new Paragraph(registro.NivelTransito)));
                    tablaHistorial.AddCell(new Cell().Add(new Paragraph($"{registro.VelocidadPromedio} km/h")));
                    tablaHistorial.AddCell(new Cell().Add(new Paragraph(registro.FechaRegistro.ToLocalTime().ToString("dd/MM/yyyy HH:mm"))));
                }

                documento.Add(tablaHistorial);
            }

            documento.Close();
            return stream.ToArray();
        }

        private Cell CeldaEncabezado(string texto)
        {
            return new Cell().Add(new Paragraph(texto).SimulateBold())
                .SetBackgroundColor(new DeviceRgb(22, 27, 34))
                .SetFontColor(new DeviceRgb(88, 166, 255));
        }
    }
}