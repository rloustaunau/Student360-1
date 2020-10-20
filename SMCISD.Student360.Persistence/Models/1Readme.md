Please Run the Following Script in order to generate the models and context. 

 > Scaffold-DbContext "Database=EdFi_Ods; Data Source=ax-edfidb\edfi; Persist Security Info=True; User Id=ed.fi; Password=M0rt0n`$1@@;" Microsoft.EntityFrameworkCore.SqlServer -Schemas "student360" -Tables "ParentUSIToStudentUSI","TeacherToStudentUsi","LocalEducationAgencyIdToStaffUSI","ParentUSIToSchoolId", "SchoolIdToStaffUSI" -DataAnnotations -OutputDir Models -ContextDir EntityFramework -Context Student360Context -Force

 NOTE: This script will overwrite the files.