// Decompiled with JetBrains decompiler
// Type: Integração.WMS.Modelos.Usuario
// Assembly: Integração WMS, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 6E836F85-7F14-4ACE-B260-A3ABAFE0C808
// Assembly location: C:\Program Files (x86)\WMS\Integração\Integração WMS.exe


namespace Integração.WMS.Modelos
{
  public class Usuario
  {
    public int id { get; set; }

    public string name { get; set; }

    public string email { get; set; }

    public string created_at { get; set; }

    public string updated_at { get; set; }

    public string tipo { get; set; }

    public int nivel_id { get; set; }

    public bool ativo { get; set; }

    public int empresa { get; set; }

    public bool rsx { get; set; }

    public bool master { get; set; }

    public bool alerta_shelf_life { get; set; }

    public string avatar { get; set; }

    public bool alerta_nota_fiscal { get; set; }

    public string session_id { get; set; }

    public bool change_password { get; set; }

    public bool roll_back { get; set; }

    public bool skip_gate { get; set; }

    public bool aplicar_todos_conferencia { get; set; }

    public bool cobranca { get; set; }

    public bool fiscal { get; set; }

    public bool edicao_pedido_item { get; set; }

    public bool comercial { get; set; }

    public bool alerta_qualidade { get; set; }
  }
}
