namespace OnlineCourse.Areas.Admin.Models
{
    public class SummerNote
    {
        public string IDEditor { get; set; }
        public bool LoadLibrary { get; set; }
        public SummerNote(string idEditor, bool loadLibrary = true)
        {
            IDEditor = idEditor;
            LoadLibrary = loadLibrary;
        }
        
        public int Height { set; get; } = 500;

        public string ToolBar { set; get; } = @"
            [
                ['style', ['style']],
                ['font', ['bold', 'italic', 'underline', 'clear']],
                ['fontname', ['fontname']],
                ['color', ['color']],
                ['para', ['ul', 'ol', 'paragraph']],
                ['table', ['table']],
                ['insert', ['link', 'elfinderFiles', 'picture', 'video', 'elfinder']],
                ['view', ['fullscreen', 'codeview', 'help']]
            ]";
    }
}