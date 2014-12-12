﻿<%@ Page Title="" Language="C#" MasterPageFile="~/Views/Shared/Site.Master" Inherits="System.Web.Mvc.ViewPage<dynamic>" %>

<asp:Content ID="Content1" ContentPlaceHolderID="TitleContent" runat="server">
	TariffsOnAbonements
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1><%: ViewData["Choose"] %></h1>
<h1 style="color:#FF0000"><%: ViewData["AbonementError"] %></h1>
   <% if (ViewData["Choose"] != "ВЫБОР АБОНЕМЕНТА")
      { %>
       <h1>АБОНЕМЕНТЫ</h1>
          <% } %>

        <fieldset>
    <h2>Тарифы, активные на текущий момент</h2>
    <table>
        <tr>
            <th></th>
            <th>
                Name_tariff_on_abonement
            </th>
            <th>
                Num_days
            </th>
            <th>
                Max_Num_visits_in_this_tariff
            </th>
            <th>
                Price
            </th>
            <th>
                Status
            </th>
        </tr>

    <% foreach (var item in (IEnumerable<ParkgMVC.Models.tariffonabonementforvisit>)ViewData["ActiveTariffs"]) {%>
                    <% using (Html.BeginForm("TariffsOnAbonements", "Tariffs", FormMethod.Post))
                       {%>
            <input type="hidden" name="Name_tariff" id="Hidden" value="<%: item.Name_tariff_on_abonement %>" />
        <tr>
            <td>
            <% if (ViewData["Choose"] == "ВЫБОР АБОНЕМЕНТА")
               {%>

                <input type="submit" name="TariffsOnAbonements" id="Connect_abonement" value="Connect_abonement"  />

                <% } %>
                            <% if (User.IsInRole("Admin"))
               { %>
                <input type="submit" name="TariffsOnAbonements" id="Submit1" value="Off_abonement"  />
               <input type="submit" name="TariffsOnAbonements" id="Submit2" value="Change_abonement"  />
                <% }%>
            </td>
            <td>
                <%: item.Name_tariff_on_abonement%>
            </td>
            <td>
                <%: item.Num_days%>
            </td>
            <td>
                <%: item.Max_Num_visits_in_this_tariff%>
            </td>
            <td>
                <%: String.Format("{0:F}", item.Price)%>
            </td>
            <td>
                <%: item.Status%>
            </td>
        </tr>
    <%} %>
    <% } %>

    </table>  
     <p>
        <%: Html.ActionLink("Create New", "Create") %>
    </p>
    </fieldset>
                <% if (ViewData["Choose"] != "ВЫБОР АБОНЕМЕНТА")
                   {%>
    <fieldset>
    <h2>Тарифы в неактивном состоянии</h2>
    <table>
        <tr>
                     <% if (User.IsInRole("Admin"))
               { %><th></th><% }%>
            <th>
                Name_tariff_on_abonement
            </th>
            <th>
                Num_days
            </th>
            <th>
                Max_Num_visits_in_this_tariff
            </th>
            <th>
                Price
            </th>
            <th>
                Status
            </th>
        </tr>

    <% foreach (var item in (IEnumerable<ParkgMVC.Models.tariffonabonementforvisit>)ViewData["NotActiveTariffs"])
       {%>
    
        <tr>
         <% if (User.IsInRole("Admin"))
               { %>
            <td>
                           
                <input type="submit" name="TariffsOnAbonements" id="Submit3" value="Run_abonement"  />
                
            </td><% }%>
            <td>
                <%: item.Name_tariff_on_abonement%>
            </td>
            <td>
                <%: item.Num_days%>
            </td>
            <td>
                <%: item.Max_Num_visits_in_this_tariff%>
            </td>
            <td>
                <%: String.Format("{0:F}", item.Price)%>
            </td>
            <td>
                <%: item.Status%>
            </td>
        </tr>
    
    <% } %>

    </table>  
</fieldset>
<% } %>
</asp:Content>
