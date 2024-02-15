// Decompiled with JetBrains decompiler
// Type: Integração.WMS.Modelos.DataEntradaRet
// Assembly: Integração WMS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6E836F85-7F14-4ACE-B260-A3ABAFE0C808
// Assembly location: C:\Program Files (x86)\WMS\Integração\Integração WMS.exe

namespace Integração.WMS.Modelos
{
  public class DataEntradaRet
  {
    public Nota_Fiscal nota_fiscal { get; set; }

    public Nota_Fiscal_Item[] nota_fiscal_item { get; set; }

    public Nr nr { get; set; }

    public Nr_Item[] nr_item { get; set; }
  }
}
