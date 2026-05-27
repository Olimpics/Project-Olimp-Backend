using OlimpBack.Application.DTO;
using System.Text;
using System.Text.Json;
using System.Net.Http.Json;

namespace OlimpBack.Application.Services;

public class GeminiService : IGeminiService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;
    private readonly string _apiKey;
    private readonly string _apiUrl;

    private const string SystemPrompt = @"
You are a specialized parser for educational elective disciplines (syllabi). 
Your task is to take a batch of JSON objects representing raw data extracted from Word files and convert them into a structured JSON array.

Input JSON format (Batch of courses):
[
  {
    ""codeAndName"": ""..."",
    ""recommendedForFields"": ""..."",
    ""department"": ""..."",
    ""instructor"": ""..."",
    ""educationLevel"": ""..."",
    ""courseAndSemester"": ""..."",
    ""languageOfInstruction"": ""..."",
    ""prerequisites"": ""..."",
    ""whyStudyThisCourse"": ""..."",
    ""topicList"": ""..."",
    ""competenciesGained"": ""..."",
    ""expectedLearningOutcomes"": ""..."",
    ""informationResources"": ""..."",
    ""typesOfLearningActivities"": ""..."",
    ""semesterControlType"": ""..."",
    ""maxMinStudents"": ""...""
  }
]

Output MUST be a valid JSON array of objects with the following structure:
[
  {
    ""CodeSelectiveDisciplines"": ""string or null"",
    ""NameSelectiveDisciplines"": ""string or null"",
    ""NameSelectiveDisciplinesEng"": ""string or null"",
    ""Recommended"": {
      ""Branches"": [""string""],
      ""Specialities"": [""string""],
      ""EducationalPrograms"": [""string""]
    },
    ""needFix"": boolean,
    ""Department"": ""string or null"",
    ""Teachers"": [""string""],
    ""DegreeLevelId"": int or null (1-Bachelor, 2-Master, etc. if identifiable),
    ""Courses"": [int] (e.g. [1, 2, 3]),
    ""IsEven"": int or null (1 for even semester, 2 for odd, or 0/1 based on logic),
    ""Language"": ""string or null"",
    ""Prerequisites"": ""string or null"",
    ""WhyInterestingDetermination"": ""string or null"",
    ""Provision"": ""string or null"",
    ""UsingIrl"": ""string or null"",
    ""ResultEducation"": ""string or null"",
    ""DisciplineTopics"": [""string""],
    ""TypesOfTraining"": ""string or null"",
    ""MinCountPeople"": int or null,
    ""MaxCountPeople"": int or null
  }
]

You are a data normalization engine for university curriculum systems. Your goal is to transform raw JSON data extracted from Word files into a strictly structured JSON array for database insertion.

### LOGIC FOR 'needFix' FIELD:
- Evaluate the 'recommendedForFields' raw input:
  - Set ""needFix"": true if the field contains any specific restrictions (e.g., branches, specialties, specific codes like ""014.04"", or faculty names).
  - Set ""needFix"": false if the field is empty, null, or contains only general phrases like ""��� ���"", ""���"", ""��� ��������"", ""��� ��� ��������������"", ""��� ��� ��������"".

### HANDLING EMPTY FIELDS:
- If a field is missing, null, or empty in the input, return `null` for single values (strings/integers) and an empty array `[]` for lists. Do not invent data.

### EXTRACTION & TRANSFORMATION RULES:

1. **CodeAndName**: Split into ""CodeSelectiveDisciplines"", ""NameSelectiveDisciplines"" (UKR), and ""NameSelectiveDisciplinesEng"" (ENG title found after the ""/"" symbol).
2. **Recommended (ONLY use 'recommendedForFields')**:
   - **Branches**: Array of strings starting with a Latin letter (A, B, C...) + title.
   - **Specialitys**: Array of strings with codes of 2-3 digits + title.
   - **EducationalPrograms**: Array of strings with codes containing dots (e.g., ""014.04"").
3. **Department**: Return only the Department's name, capitalized. Remove prefixes like ""�������"".
4. **Teachers**: Return an array of strings. Keep the FULL NAME (Last, First, Middle). Remove academic titles (����., ���., �.�.�., PhD, etc.).
5. **DegreeLevelId**: Map input: ""��������"" -> 1, ""������"" -> 2, ""�������"" -> 3. Otherwise `null`.
6. **Courses**: Array of integers (e.g., [1, 2, 3]).
7. **IsEven**: If only even semesters mentioned -> 1; only odd -> 0; both or unspecified -> null.
8. **DisciplineTopics**: Convert the text list into a clean array of strings (one per topic).
9. **Count Constraints**: Split ""60/15"" into ""MaxCountPeople"": 60 and ""MinCountPeople"": 15. If only one number ""100"" is provided, set Min to `null`.
10. **Text Cleaning**: For fields (Prerequisites, WhyInteresting, Provision, UsingIrl, ResultEducation, TypesOfTraining), remove line breaks (\n), fix broken words, and ensure clean prose.

### OUTPUT FORMAT:
Return ONLY a valid JSON array of objects. No markdown, no conversational filler, no explanations.
";

    private const string EducationalProgramSystemPrompt = @"
You are a university curriculum expert. Your task is to process raw data from an Educational Program (OP) document and normalize it into a structured JSON format.

### Input Data Format:
{
  ""nameEducationalProgram"": ""..."",
  ""degree"": ""..."",
  ""studyForm"": ""..."",
  ""goals"": ""..."",
  ""specialityAndSpecializationWithDetails"": ""..."",
  ""subject"": ""..."",
  ""mainDisciplines"": [ { ""code"": ""..."", ""name"": ""..."", ""loans"": ""..."", ""control"": ""..."", ""semester"": ""..."" } ],
  ""mainDisciplinesNeedFix"": [ ""string1 | string2 | ..."", ""..."" ],
  ""selectiveDisciplines"": [ { ""code"": ""..."", ""name"": ""..."", ""loans"": ""..."", ""control"": ""..."", ""semester"": ""..."" } ]
}

### Output MUST be a valid JSON object with the following structure:
{
  ""NameEducationalProgram"": ""string"",
  ""Degree"": ""string (single word: 'Бакалавр', 'Магістр', or 'Аспірант')"",
  ""StudyForm"": ""string (MUST be one of: 'Денна', 'Заочна', 'Дистанційна', 'Вечірня', 'Дуальна')"",
  ""Subject"": ""string"",
  ""Speciality"": ""string (code + title, e.g., '122 Комп'ютерні науки')"",
  ""Specialization"": ""string or null"",
  ""Goals"": ""string"",
  ""MainDisciplines"": [
    {
      ""Code"": ""string"",
      ""Name"": ""string (normalized, clean title)"",
      ""Loans"": int or null,
      ""Control"": ""string"",
      ""Semester"": int or null
    }
  ],
  ""SelectiveDisciplines"": [
    {
      ""Code"": ""string"",
      ""Name"": ""string (normalized, clean title)"",
      ""Loans"": int or null,
      ""Control"": ""string"",
      ""Semester"": int or null
    }
  ],
  ""SelectiveDisciplineBySemestr"": [int] (array representing elective discipline count per semester)
}

### NORMALIZATION RULES:
1. **Name/Title Cleaning**: 
   - **Educational Program Name**: Extract ONLY the specific name of the program. Remove boilerplate prefixes like ""Освітньо-професійна програма"", ""Освітньо-наукова програма"", or quotes if they wrap the entire name. 
     - *Example*: ""Освітньо-професійна програма «Технології медичної діагностики та лікування»"" -> ""Технології медичної діагностики та лікування"".
   - **Discipline Names**: Clean redundant spaces, line breaks, and ensure only the course title remains.
2. **MainDisciplinesNeedFix Processing**: 
   - These are raw strings from rows where column alignment failed (e.g., the Name bled into the Loans field). The fields are separated by "" | "". 
   - You MUST intelligently parse these strings to restore the correct Code, Name, Loans, Control, and Semester.
   - After fixing, include them in the `MainDisciplines` output array.
3. **Disciplines splitting (Main and Selective)**: 
   - If a discipline (main or selective) has multiple semesters (e.g., ""1, 2, 4""), create a separate record for each semester with the same Code, Name, and Loans.
   - **Assessment (Control) Splitting**: If multiple assessment types are listed (e.g., ""екзамен, залік"") corresponding to multiple semesters, you MUST pair them correctly. 
     - *Example*: Name: ""Math"", Semester: ""1, 2"", Control: ""екзамен залік"" -> Record 1: Semester: 1, Control: ""екзамен""; Record 2: Semester: 2, Control: ""залік"".
     - If only one assessment type is listed for multiple semesters, use it for all of them.
4. **SelectiveDisciplineBySemestr calculation**:
   - Count the number of selective disciplines for each semester based on the 'selectiveDisciplines' input.
   - The array index corresponds to (Semester - 1).
   - The length of this array MUST be exactly equal to the MAXIMUM semester number found in the final processed 'MainDisciplines'.
   - If a semester has no elective courses, fill with 0.
   - Example: If max semester in 'MainDisciplines' is 3, and there are 5 elective courses in semester 2, the array should be [0, 5, 0].
5. **Speciality/Specialization**: Extract these from 'specialityAndSpecializationWithDetails'.
6. **Text Cleaning**: For all text fields, fix broken words (e.g., ""лабора- торних"" -> ""лабораторних"") and ensure professional prose.
7. **Degree**: Return a single word. If the text mentions a master (магістр), return ""Магістр"". If bachelor (бакалавр) -> ""Бакалавр"". If postgraduate (аспірант) -> ""Аспірант"".
8. **Numeric Fields**: 'Loans' and 'Semester' MUST be returned as integers. Convert strings like ""3,0"" or ""3.0"" to the integer 3.
9. **Column Separation (CRITICAL)**: Treat each field in the output objects as atomic. Ensure that `SelectiveDisciplines` are processed with the exact same rigor as `MainDisciplines`.

Return ONLY valid JSON.
";

    public GeminiService(HttpClient httpClient, IConfiguration configuration)
    {
        _httpClient = httpClient;
        _httpClient.Timeout = TimeSpan.FromMinutes(7);
        _configuration = configuration;
        _apiKey = _configuration["Gemini:ApiKey"] ?? "";
        _apiUrl = $"https://generativelanguage.googleapis.com/v1beta/models/gemini-2.5-flash:generateContent?key={_apiKey}";
    }

    public async Task<GeminiEducationalProgramDto?> ProcessEducationalProgramAsync(EducationalProgramWordContentDto content)
    {
        var inputJson = JsonSerializer.Serialize(content);
        var prompt = $"{EducationalProgramSystemPrompt}\n\nInput Data:\n{inputJson}";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            },
            generationConfig = new
            {
                response_mime_type = "application/json"
            }
        };

        var response = await _httpClient.PostAsJsonAsync(_apiUrl, requestBody);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Gemini API error: {response.StatusCode} - {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<GeminiResponse>();
        var jsonResponse = result?.Candidates?[0]?.Content?.Parts?[0]?.Text;

        if (string.IsNullOrWhiteSpace(jsonResponse))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<GeminiEducationalProgramDto>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
        catch (JsonException)
        {
            var cleanedJson = CleanJson(jsonResponse);
            return JsonSerializer.Deserialize<GeminiEducationalProgramDto>(cleanedJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
        }
    }

    public async Task<List<GeminiSelectiveDisciplineDto>> ProcessSelectiveDisciplinesAsync(List<SelectiveDisciplineWordContentDto> batch)
    {
        var inputJson = JsonSerializer.Serialize(batch);
        var prompt = $"{SystemPrompt}\n\nInput Data:\n{inputJson}";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            },
            generationConfig = new
            {
                response_mime_type = "application/json"
            }
        };

        var response = await _httpClient.PostAsJsonAsync(_apiUrl, requestBody);
        
        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Gemini API error: {response.StatusCode} - {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<GeminiResponse>();
        var jsonResponse = result?.Candidates?[0]?.Content?.Parts?[0]?.Text;

        if (string.IsNullOrWhiteSpace(jsonResponse))
        {
            return new List<GeminiSelectiveDisciplineDto>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<GeminiSelectiveDisciplineDto>>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<GeminiSelectiveDisciplineDto>();
        }
        catch (JsonException ex)
        {
            // Fallback: try to clean markdown if Gemini added it despite instructions
            var cleanedJson = CleanJson(jsonResponse);
            return JsonSerializer.Deserialize<List<GeminiSelectiveDisciplineDto>>(cleanedJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<GeminiSelectiveDisciplineDto>();
        }
    }

    public async Task<List<GeminiEducationalProgramDto>> ProcessEducationalProgramsAsync(List<EducationalProgramWordContentDto> batch)
    {
        var inputJson = JsonSerializer.Serialize(batch);
        var prompt = $"{EducationalProgramSystemPrompt}\n\nInput Data:\n{inputJson}\n\nReturn a JSON array of objects.";

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = prompt }
                    }
                }
            },
            generationConfig = new
            {
                response_mime_type = "application/json"
            }
        };

        var response = await _httpClient.PostAsJsonAsync(_apiUrl, requestBody);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            throw new Exception($"Gemini API error: {response.StatusCode} - {errorContent}");
        }

        var result = await response.Content.ReadFromJsonAsync<GeminiResponse>();
        var jsonResponse = result?.Candidates?[0]?.Content?.Parts?[0]?.Text;

        if (string.IsNullOrWhiteSpace(jsonResponse))
        {
            return new List<GeminiEducationalProgramDto>();
        }

        try
        {
            return JsonSerializer.Deserialize<List<GeminiEducationalProgramDto>>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<GeminiEducationalProgramDto>();
        }
        catch (JsonException)
        {
            var cleanedJson = CleanJson(jsonResponse);
            return JsonSerializer.Deserialize<List<GeminiEducationalProgramDto>>(cleanedJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            }) ?? new List<GeminiEducationalProgramDto>();
        }
    }

    private string CleanJson(string json)
    {
        if (json.StartsWith("```json"))
        {
            json = json.Substring(7);
        }
        if (json.EndsWith("```"))
        {
            json = json.Substring(0, json.Length - 3);
        }
        return json.Trim();
    }

    private class GeminiResponse
    {
        public Candidate[]? Candidates { get; set; }
    }

    private class Candidate
    {
        public Content? Content { get; set; }
    }

    private class Content
    {
        public Part[]? Parts { get; set; }
    }

    private class Part
    {
        public string? Text { get; set; }
    }
}
