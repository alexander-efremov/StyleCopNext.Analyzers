// Copyright (c) Tunnel Vision Laboratories, LLC. All Rights Reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

#nullable disable

namespace StyleCop.Analyzers.Settings.ObjectModel
{
    using LightJson;
    using Microsoft.CodeAnalysis.Diagnostics;

    internal class IndentationSettings
    {
        /// <summary>
        /// This is the backing field for the <see cref="IndentationSize"/> property.
        /// </summary>
        private readonly int indentationSize;

        /// <summary>
        /// This is the backing field for the <see cref="TabSize"/> property.
        /// </summary>
        private readonly int tabSize;

        /// <summary>
        /// This is the backing field for the <see cref="UseTabs"/> property.
        /// </summary>
        private readonly bool useTabs;

        /// <summary>
        /// This is the backing field for the <see cref="IndentBlock"/> property.
        /// </summary>
        private readonly bool indentBlock;

        /// <summary>
        /// This is the backing field for the <see cref="IndentSwitchSection"/> property.
        /// </summary>
        private readonly bool indentSwitchSection;

        /// <summary>
        /// This is the backing field for the <see cref="IndentSwitchCaseSection"/> property.
        /// </summary>
        private readonly bool indentSwitchCaseSection;

        /// <summary>
        /// This is the backing field for the <see cref="LabelPositioning"/> property.
        /// </summary>
        private readonly LabelPositioning labelPositioning;

        /// <summary>
        /// Initializes a new instance of the <see cref="IndentationSettings"/> class.
        /// </summary>
        protected internal IndentationSettings()
        {
            this.indentationSize = 4;
            this.tabSize = 4;
            this.useTabs = false;
            this.indentBlock = true;
            this.indentSwitchSection = true;
            this.indentSwitchCaseSection = true;
            this.labelPositioning = LabelPositioning.OneLess;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="IndentationSettings"/> class.
        /// </summary>
        /// <param name="indentationSettingsObject">The JSON object containing the settings.</param>
        /// <param name="analyzerConfigOptions">The <strong>.editorconfig</strong> options to use if
        /// <strong>stylecop.json</strong> does not provide values.</param>
        protected internal IndentationSettings(JsonObject indentationSettingsObject, AnalyzerConfigOptions analyzerConfigOptions)
        {
            int? indentationSize = null;
            int? tabSize = null;
            bool? useTabs = null;
            bool? indentBlock = null;
            bool? indentSwitchSection = null;
            bool? indentSwitchCaseSection = null;
            LabelPositioning? labelPositioning = null;

            foreach (var kvp in indentationSettingsObject)
            {
                switch (kvp.Key)
                {
                case "indentationSize":
                    indentationSize = kvp.ToInt32Value();
                    break;

                case "tabSize":
                    tabSize = kvp.ToInt32Value();
                    break;

                case "useTabs":
                    useTabs = kvp.ToBooleanValue();
                    break;

                case "indentBlock":
                    indentBlock = kvp.ToBooleanValue();
                    break;

                case "indentSwitchSection":
                    indentSwitchSection = kvp.ToBooleanValue();
                    break;

                case "indentSwitchCaseSection":
                    indentSwitchCaseSection = kvp.ToBooleanValue();
                    break;

                case "labelPositioning":
                    labelPositioning = kvp.ToEnumValue<LabelPositioning>();
                    break;

                default:
                    break;
                }
            }

            indentationSize ??= AnalyzerConfigHelper.TryGetInt32Value(analyzerConfigOptions, "indent_size");
            tabSize ??= AnalyzerConfigHelper.TryGetInt32Value(analyzerConfigOptions, "tab_width");
            useTabs ??= AnalyzerConfigHelper.TryGetStringValue(analyzerConfigOptions, "indent_style") switch
            {
                "tab" => true,
                "space" => false,
                _ => null,
            };

            this.indentationSize = indentationSize.GetValueOrDefault(4);
            this.tabSize = tabSize.GetValueOrDefault(4);
            this.useTabs = useTabs.GetValueOrDefault(false);
            this.indentBlock = indentBlock.GetValueOrDefault(true);
            this.indentSwitchSection = indentSwitchSection.GetValueOrDefault(true);
            this.indentSwitchCaseSection = indentSwitchCaseSection.GetValueOrDefault(true);
            this.labelPositioning = labelPositioning.GetValueOrDefault(LabelPositioning.OneLess);
        }

        public int IndentationSize =>
            this.indentationSize;

        public int TabSize =>
            this.tabSize;

        public bool UseTabs =>
            this.useTabs;

        public bool IndentBlock =>
            this.indentBlock;

        public bool IndentSwitchSection =>
            this.indentSwitchSection;

        public bool IndentSwitchCaseSection =>
            this.indentSwitchCaseSection;

        public LabelPositioning LabelPositioning =>
            this.labelPositioning;
    }
}
