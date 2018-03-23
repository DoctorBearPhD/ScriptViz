using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace ScriptViz.Behaviours
{
    /// <inheritdoc/>
    /// <summary>
    /// 
    /// </summary>
    public class DataGridBehaviour : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            AssociatedObject.AutoGeneratingColumn += OnAutoGeneratingColumn;
        }

        protected override void OnDetaching()
        {
            AssociatedObject.AutoGeneratingColumn -= OnAutoGeneratingColumn;
        }

        protected void OnAutoGeneratingColumn(object sender, DataGridAutoGeneratingColumnEventArgs e)
        {
            // Manipulate Column

            var propdesc = e.PropertyDescriptor;

            if (!(propdesc is PropertyDescriptor pd)) return;

            // If Browsable Attribute is false, don't show this column.
            if (!pd.IsBrowsable)
            {
                e.Cancel = true;
                return;
            }

            var proptype = e.PropertyType;
            var prop = e.Column;

            if (pd.DisplayName != e.PropertyName)
            {
                e.Column.Header = pd.DisplayName;
            }

            if (pd.Description != "")
            {
                var tooltipSetter = new Setter(FrameworkElement.ToolTipProperty, pd.Description);

                if (e.Column.CellStyle == null)
                {
                    var style = new Style(typeof(DataGridCell));
                    style.Setters.Add(tooltipSetter);
                    e.Column.CellStyle = style;
                }
                else
                    e.Column.CellStyle.Setters.Add(tooltipSetter);
            }

            //var p = pd.Attributes.OfType<DisplayAttribute>();

            //var displayAttributes = p.ToList();

            //if (displayAttributes.Any())
            //{
            //    foreach (var attr in displayAttributes)
            //    {
            //        if (attr.GetOrder() != null)
            //        {
            //            e.Column.DisplayIndex = attr.Order;
            //        }
            //    }
            //}
        }
    }
}
