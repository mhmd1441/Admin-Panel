using System.Net.Http.Json;
using AdminPanel.Client.Contracts;

namespace AdminPanel.Client.Services;

public class ApiClient(HttpClient http)
{
    public async Task<List<ProjectSummaryDto>> GetProjectsAsync() =>
        await http.GetFromJsonAsync<List<ProjectSummaryDto>>("api/projects") ?? [];

    public async Task<ProjectDetailDto?> GetProjectAsync(int id) =>
        await http.GetFromJsonAsync<ProjectDetailDto>($"api/projects/{id}");

    public async Task<ProjectDetailDto?> CreateProjectAsync(ProjectUpsertRequest request)
    {
        var response = await http.PostAsJsonAsync("api/projects", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProjectDetailDto>();
    }

    public async Task<ProjectDetailDto?> UpdateProjectAsync(int id, ProjectUpsertRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/projects/{id}", request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<ProjectDetailDto>();
    }

    public async Task DeleteProjectAsync(int id)
    {
        var response = await http.DeleteAsync($"api/projects/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task CreateContactAsync(int projectId, ContactUpsertRequest request)
    {
        var response = await http.PostAsJsonAsync($"api/projects/{projectId}/contacts", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteContactAsync(int id)
    {
        var response = await http.DeleteAsync($"api/contacts/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task CreateInvoiceAsync(int projectId, InvoiceUpsertRequest request)
    {
        var response = await http.PostAsJsonAsync($"api/projects/{projectId}/invoices", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateInvoiceAsync(int id, InvoiceUpsertRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/invoices/{id}", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteInvoiceAsync(int id)
    {
        var response = await http.DeleteAsync($"api/invoices/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task CreateNoteAsync(int projectId, NoteCreateRequest request)
    {
        var response = await http.PostAsJsonAsync($"api/projects/{projectId}/notes", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteNoteAsync(int id)
    {
        var response = await http.DeleteAsync($"api/notes/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task CreateRequirementAsync(int projectId, RequirementUpsertRequest request)
    {
        var response = await http.PostAsJsonAsync($"api/projects/{projectId}/requirements", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task UpdateRequirementAsync(int id, RequirementUpsertRequest request)
    {
        var response = await http.PutAsJsonAsync($"api/requirements/{id}", request);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteRequirementAsync(int id)
    {
        var response = await http.DeleteAsync($"api/requirements/{id}");
        response.EnsureSuccessStatusCode();
    }

    public async Task UploadDocumentAsync(int projectId, MultipartFormDataContent content)
    {
        var response = await http.PostAsync($"api/projects/{projectId}/documents", content);
        response.EnsureSuccessStatusCode();
    }

    public async Task DeleteDocumentAsync(int id)
    {
        var response = await http.DeleteAsync($"api/documents/{id}");
        response.EnsureSuccessStatusCode();
    }
}
