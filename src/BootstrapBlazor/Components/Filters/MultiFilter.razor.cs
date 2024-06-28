﻿// Copyright (c) Argo Zhang (argo@163.com). All rights reserved.
// Licensed under the Apache License, Version 2.0. See License.txt in the project root for license information.
// Website: https://www.blazor.zone or https://argozhang.github.io/

namespace BootstrapBlazor.Components;

/// <summary>
/// 表格过滤菜单组件
/// </summary>
public partial class MultiFilter
{
    /// <summary>
    /// 获得/设置 搜索栏占位符 默认 nul 使用资源文件中值
    /// </summary>
    [Parameter]
    public string? SearchPlaceHolderText { get; set; }

    /// <summary>
    /// 获得/设置 全选按钮文本 默认 nul 使用资源文件中值
    /// </summary>
    [Parameter]
    public string? SelectAllText { get; set; }

    /// <summary>
    /// 获得/设置 是否显示搜索栏 默认 true
    /// </summary>
    [Parameter]
    public bool ShowSearch { get; set; } = true;

    private string? _searchText;

    private List<MultiFilterItem> _source = [];

    private List<MultiFilterItem>? _items;

    /// <summary>
    /// OnInitialized 方法
    /// </summary>
    protected override void OnInitialized()
    {
        base.OnInitialized();
        if (TableFilter != null)
        {
            TableFilter.ShowMoreButton = false;
        }
    }

    /// <summary>
    /// <inheritdoc/>
    /// </summary>
    protected override void OnParametersSet()
    {
        base.OnParametersSet();

        SearchPlaceHolderText ??= Localizer["MultiFilterSearchPlaceHolderText"];
        SelectAllText ??= Localizer["MultiFilterSelectAllText"];

        if (Items != null)
        {
            _source = Items.Select(item => new MultiFilterItem() { Value = item.Value, Text = item.Text }).ToList();
        }
    }

    /// <summary>
    /// 重置过滤条件方法
    /// </summary>
    public override void Reset()
    {
        _searchText = string.Empty;
        foreach (var item in _source)
        {
            item.Checked = false;
        }
        StateHasChanged();
    }

    /// <summary>
    /// 生成过滤条件方法
    /// </summary>
    /// <returns></returns>
    public override FilterKeyValueAction GetFilterConditions()
    {
        var filter = new FilterKeyValueAction() { Filters = [], FilterLogic = FilterLogic.Or };

        foreach (var item in GetItems().Where(i => i.Checked))
        {
            filter.Filters.Add(new FilterKeyValueAction()
            {
                FieldKey = FieldKey,
                FieldValue = item.Value,
                FilterAction = FilterAction.Equal
            });
        }
        return filter;
    }

    private CheckboxState _selectAllState = CheckboxState.UnChecked;

    private CheckboxState GetState()
    {
        var state = CheckboxState.UnChecked;
        var items = GetItems();
        if (items.Count > 0)
        {
            state = items.All(i => i.Checked)
                ? CheckboxState.Checked
                : items.Any(i => i.Checked)
                    ? CheckboxState.Indeterminate
                    : CheckboxState.UnChecked;
        }
        return state;
    }

    private bool GetAllState()
    {
        _selectAllState = GetState();
        return _selectAllState == CheckboxState.Checked;
    }

    private Task OnStateChanged(CheckboxState state, bool val)
    {
        foreach (var item in _source)
        {
            item.Checked = state == CheckboxState.Checked;
        }
        StateHasChanged();
        return Task.CompletedTask;
    }

    /// <summary>
    /// 过滤内容搜索
    /// </summary>
    /// <param name="val"></param>
    /// <returns></returns>
    private Task OnSearchValueChanged(string? val)
    {
        _searchText = val;
        if (!string.IsNullOrEmpty(_searchText))
        {
            _items = _source.Where(i => i.Text.Contains(_searchText)).ToList();
        }
        else
        {
            _items = null;
        }
        StateHasChanged();
        return Task.CompletedTask;
    }

    private List<MultiFilterItem> GetItems() => _items ?? _source;

    class MultiFilterItem
    {
        public bool Checked { get; set; }

        [NotNull]
        public string? Value { get; init; }

        [NotNull]
        public string? Text { get; init; }
    }
}