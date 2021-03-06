﻿using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace df
{
    public class DisplayNameDf : DisplayNameAttribute
    {
        public DisplayNameDf(string str) : base(str)
        {

        }

        public override string DisplayName
        {
            get
            {
                return AppLanguage.getLang(base.DisplayNameValue);
            }
        }
    }

    public class DescriptionDf : DescriptionAttribute
    {
        public DescriptionDf(string str) : base(str)
        {

        }
        public override string Description
        {
            get
            {
                return AppLanguage.getLang(base.DescriptionValue);
            }
        }
    }

    public class CategoryDf : CategoryAttribute
    {
        public CategoryDf(string category) : base(category)
        {

        }

        protected override string GetLocalizedString(string value)
        {
            return AppLanguage.getLang(value);
        }
    }

    public class PercentConverter : ComboBoxItemTypeConvert
    {
        public override Dictionary<string, object> GetConvertMap()
        {
            var dict = new Dictionary<string, object>() {
                { "0" , 0 },
                { "10" , 10 },
                { "20" , 20 },
                 { "30" , 30 },
                 { "40" , 40 },
                 { "50" , 50 },
                 { "60" , 60 },
                 { "70" , 70 },
                 { "80" , 80 },
                 { "90" , 90 },
                 { "100" , 100 },
            };
            return dict;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }
    }

    public class FloatConverter : ComboBoxItemTypeConvert
    {
        public override Dictionary<string, object> GetConvertMap()
        {
            var dict = new Dictionary<string, object>() {
                { "0" , 0 },
                { "1" , 1 },
                { "2" , 2 },
                { "3" , 3 },
                { "4" , 4 },
                { "5" ,5 },
                { "6" , 6 },
                { "7" , 7 },
                { "8" , 8 },
                { "9" , 9 },
                { "10" , 10 },
            };
            return dict;
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }
    }

    public class NumberConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new string[] { "", "1", "2", "3", "4", "5", "6", "7", "8", "9" });
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return false;
        }
    }

    public class PropertyGridColor : UITypeEditor
    {

        public PropertyGridColor()
        {
        }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;

        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            ColorDialog ColorForm = new ColorDialog();
            if (ColorForm.ShowDialog() == DialogResult.OK)
            {
                var c = ColorForm.Color;
                return "0x" + c.R.ToString("X2") + c.G.ToString("X2") + c.B.ToString("X2");
            }

            return value;

        }

    }

    /// <summary>
    ///  Crop Video on PropertyGrid
    /// </summary>
    public class PropertyGridVideoCrop : UITypeEditor
    {

        public PropertyGridVideoCrop()
        {
        }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;

        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {
            var inst = context.Instance;
            if (!(inst is FileConvertParameter))
            {
                var res = context.GetType().GetProperty("Parent");
                if (res != null)
                {
                    var con = res.GetValue(context) as System.ComponentModel.ITypeDescriptorContext;
                    inst = con.Instance;
                }
            }

            if (inst is FileConvertParameter)
            {
                var para = inst as FileConvertParameter;
                var fp = new FormPlayer();
                if (fp.cropStart(para.fileName))
                {
                    return JsonConvert.DeserializeObject(fp.getSelectedRectStr(), value.GetType());
                }
            }


            var grid = context.getGrid();
            if (grid != null && grid.Tag is List<FFmpeg>)
            {
                var para = grid.Tag as List<FFmpeg>;
                if (para.Count < 1)
                {
                    throw new ExceptionFFmpeg(dfv.lang.dat.HaveToAddFile);
                }
                var fp = new FormPlayer();
                if (fp.cropStart(para[0].fileName))
                {
                    return JsonConvert.DeserializeObject(fp.getSelectedRectStr(), value.GetType());
                }
            }

            return value;

        }

    }

    /// <summary>
    ///  Show Video on PropertyGrid
    /// </summary>
    public class PropertyGridVideoTime : UITypeEditor
    {


        public PropertyGridVideoTime()
        {
        }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;

        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {

            if (context.Instance is FileConvertParameter)
            {
                var para = context.Instance as FileConvertParameter;
                var fp = new FormPlayer();
                var time = fp.selectTime(para.fileName);
                if (time != "")
                    return time;
            }

            return value;

        }

    }




    public class PropertyGridRichText : UITypeEditor
    {

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {

            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc != null)
            {
                if (value is string)
                {
                    RichTextBox box = new RichTextBox();
                    box.Text = value as string;
                    edSvc.DropDownControl(box);
                    return box.Text;
                }
            }
            return value;

        }

    }

    public class PropertyListAttribute : Attribute
    {
        public string[] list { get; set; } = new string[] { };
        public bool exclusive { get; set; } = true;
    }

    public class ListConverter : StringConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var list = context.PropertyDescriptor.Attributes.Find<PropertyListAttribute>();
            return new StandardValuesCollection(list.list);
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            var list = context.PropertyDescriptor.Attributes.Find<PropertyListAttribute>();
            return list.exclusive;
        }
    }

    public class PropertySlideAttribute : Attribute
    {
        public int defaul { get; set; } = 0;
        public int max { get; set; } = 10;
        public int min { get; set; } = 0;
        public int step { get; set; } = 1;
        public int toFloat { get; set; } = 1;
    }

    public class PropertyGridSlide : UITypeEditor
    {

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {

            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (edSvc != null)
            {
                if (value is string)
                {
                    PropertySlideAttribute att = context.PropertyDescriptor.Attributes.Find<PropertySlideAttribute>();
                    SlideText box = new SlideText();
                    box.ValueStr = value as string;
                    box.initFromAtt(att);
                    edSvc.DropDownControl(box);
                    return box.ValueStr;
                }
            }
            return value;

        }

    }

    /// <summary>
    ///  Show Calendar on PropertyGrid
    /// </summary>
    public class PropertyGridDateItem : UITypeEditor
    {

        MonthCalendar dateControl = new MonthCalendar();

        public PropertyGridDateItem()
        {
            dateControl.MaxSelectionCount = 1;
        }

        public override UITypeEditorEditStyle GetEditStyle(System.ComponentModel.ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;

        }

        public override object EditValue(System.ComponentModel.ITypeDescriptorContext context, System.IServiceProvider provider, object value)
        {

            IWindowsFormsEditorService edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));

            if (edSvc != null)

            {

                if (value is string)
                {

                    dateControl.SelectionStart = DateTime.Parse(value as String);

                    edSvc.DropDownControl(dateControl);

                    return dateControl.SelectionStart.ToShortDateString();

                }

                else if (value is DateTime)
                {
                    dateControl.SelectionStart = (DateTime)value;
                    edSvc.DropDownControl(dateControl);
                    return dateControl.SelectionStart;
                }

            }


            return value;

        }

    }




    public class YesNoConverter : ComboBoxItemTypeConvert
    {
        public override Dictionary<string, object> GetConvertMap()
        {
            return new Dictionary<string, object>() {
                { dfv.lang.dat.Yes , "1" },
                { dfv.lang.dat.No , "" },
            };
        }
    }

    public class YesNoDefaultConverter : ComboBoxItemTypeConvert
    {
        public override Dictionary<string, object> GetConvertMap()
        {
            return new Dictionary<string, object>() {
                { "" , "" },
                { dfv.lang.dat.Yes , "1" },
                { dfv.lang.dat.No , "0" },
            };
        }
    }

    public abstract class ComboBoxItemTypeConvert : TypeConverter
    {
        public Dictionary<string, object> _hash = null;
        public ComboBoxItemTypeConvert()
        {
            _hash = GetConvertMap();
        }

        public abstract Dictionary<string, object> GetConvertMap();

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(_hash.Select(it => it.Value).ToList());
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (context == null)
                return false;
            if (destinationType == typeof(string))
            {
                return true;
            }
            return base.CanConvertTo(context, destinationType);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (context == null)
                return false;
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        /// <summary>
        /// Convert Combox key to Object value
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="v"></param>
        /// <returns></returns>
        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object v)
        {
            if (v is string)
            {
                if (_hash.ContainsKey(v as string))
                    return _hash[v as string];

                if (context.PropertyDescriptor.PropertyType == typeof(int))
                {
                    return int.Parse(v + "");
                }
                else if (context.PropertyDescriptor.PropertyType == typeof(float))
                {
                    return float.Parse(v + "");
                }

                return v;
            }
            return base.ConvertFrom(context, culture, v);
        }

        /// <summary>
        /// show Object value to Combox list
        /// </summary>
        /// <param name="context"></param>
        /// <param name="culture"></param>
        /// <param name="v"></param>
        /// <param name="destinationType"></param>
        /// <returns></returns>
        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object v, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                foreach (var myDE in _hash)
                {
                    if (myDE.Value.Equals(v))
                        return myDE.Key;
                }

                return v + "";
            }
            return base.ConvertTo(context, culture, v, destinationType);
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }
    }


    public class SubClassConverter : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (context == null)
                return false;
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (context == null)
                return false;
            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                //if (value != null)
                //{
                //    return value.GetType().GetProperties().JoinStr(" , ", it =>
                //    {
                //        if (it.PropertyType != typeof(string))
                //            return null;
                //        var val = it.GetValue(value) + "";
                //        return val == "" ? null : val;
                //    });
                //}
                return "";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

    public class SubClassJSONConverter : ExpandableObjectConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (context == null)
                return false;

            if (sourceType == typeof(string))
                return true;
            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (context == null)
                return false;

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                var res = JsonConvert.DeserializeObject(value as string, context.PropertyDescriptor.PropertyType);
                if (res == null)
                {
                    res = JsonConvert.DeserializeObject("{}", context.PropertyDescriptor.PropertyType);
                }
                return res;
            }
            return base.ConvertFrom(context, culture, value);
        }


        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                if (value != null)
                {
                    return "{" + value.GetType().GetProperties().JoinStr(",", it =>
                        {
                            var val = it.GetValue(value);
                            if (val != null && val.ToString() != "")
                            {
                                return it.Name + ":" + val;
                            }
                            return null;
                        }) + " }";
                }
                return "{ }";
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    }

}
