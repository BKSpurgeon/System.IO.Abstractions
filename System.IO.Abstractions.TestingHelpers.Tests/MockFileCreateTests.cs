namespace System.IO.Abstractions.TestingHelpers.Tests
{
    using Collections.Generic;

    using Globalization;

    using NUnit.Framework;
    using System.Security.AccessControl;
    using Text;

    using XFS = MockUnixSupport;

    public class MockFileCreateTests
    {
        [Test]
        public void Mockfile_Create_ShouldCreateNewStream()
        {
            string fullPath = XFS.Path(@"c:\something\demo.txt");
            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(XFS.Path(@"c:\something"));

            var sut = new MockFile(fileSystem);

            Assert.That(fileSystem.FileExists(fullPath), Is.False);

            sut.Create(fullPath).Dispose();

            Assert.That(fileSystem.FileExists(fullPath), Is.True);
        }

        [Test]
        public void Mockfile_Create_CanWriteToNewStream()
        {
            string fullPath = XFS.Path(@"c:\something\demo.txt");
            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(XFS.Path(@"c:\something"));
            var data = new UTF8Encoding(false).GetBytes("Test string");

            var sut = new MockFile(fileSystem);
            using (var stream = sut.Create(fullPath))
            {
                stream.Write(data, 0, data.Length);
            }

            var mockFileData = fileSystem.GetFile(fullPath);
            var fileData = mockFileData.Contents;

            Assert.That(fileData, Is.EqualTo(data));
        }

        [Test]
        public void Mockfile_Create_OverwritesExistingFile()
        {
            string path = XFS.Path(@"c:\some\file.txt");
            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(XFS.Path(@"c:\some"));

            var mockFile = new MockFile(fileSystem);

            // Create a file
            using (var stream = mockFile.Create(path))
            {
                var contents = new UTF8Encoding(false).GetBytes("Test 1");
                stream.Write(contents, 0, contents.Length);
            }

            // Create new file that should overwrite existing file
            var expectedContents = new UTF8Encoding(false).GetBytes("Test 2");
            using (var stream = mockFile.Create(path))
            {
                stream.Write(expectedContents, 0, expectedContents.Length);
            }

            var actualContents = fileSystem.GetFile(path).Contents;

            Assert.That(actualContents, Is.EqualTo(expectedContents));
        }

        [Test]
        public void Mockfile_Create_ShouldThrowUnauthorizedAccessExceptionIfPathIsReadOnly()
        {
            // Arrange
            string path = XFS.Path(@"c:\something\read-only.txt");
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { { path, new MockFileData("Content") } });
            var mockFile = new MockFile(fileSystem);

            // Act
            mockFile.SetAttributes(path, FileAttributes.ReadOnly);

            // Assert
            var exception = Assert.Throws<UnauthorizedAccessException>(() => mockFile.Create(path).Dispose());
            Assert.That(exception.Message, Is.EqualTo(string.Format(CultureInfo.InvariantCulture, "Access to the path '{0}' is denied.", path)));
        }

        [Test]
        public void Mockfile_Create_ShouldThrowArgumentExceptionIfPathIsZeroLength()
        {
            // Arrange
            var fileSystem = new MockFileSystem();

            // Act
            TestDelegate action = () => fileSystem.File.Create("");

            // Assert
            Assert.Throws<ArgumentException>(action);
        }

        [TestCase("\"")]
        [TestCase("<")]
        [TestCase(">")]
        [TestCase("|")]
        [WindowsOnly(WindowsSpecifics.StrictPathRules)]
        public void MockFile_Create_ShouldThrowArgumentNullExceptionIfPathIsNull1(string path)
        {
            // Arrange
            var fileSystem = new MockFileSystem();

            // Act
            TestDelegate action = () => fileSystem.File.Create(path);

            // Assert
            Assert.Throws<ArgumentException>(action);
        }

        [TestCase(" ")]
        [TestCase("   ")]
        public void MockFile_Create_ShouldThrowArgumentExceptionIfPathContainsOnlyWhitespaces(string path)
        {
            // Arrange
            var fileSystem = new MockFileSystem();

            // Act
            TestDelegate action = () => fileSystem.File.Create(path);

            // Assert
            Assert.Throws<ArgumentException>(action);
        }

        [Test]
        public void MockFile_Create_ShouldThrowArgumentNullExceptionIfPathIsNull()
        {
            // Arrange
            var fileSystem = new MockFileSystem();

            // Act
            TestDelegate action = () => fileSystem.File.Create(null);

            // Assert
            var exception = Assert.Throws<ArgumentNullException>(action);
            Assert.That(exception.Message, Does.StartWith("Path cannot be null."));
        }

        [Test]
        public void MockFile_Create_ShouldThrowDirectoryNotFoundExceptionIfCreatingAndParentPathDoesNotExist()
        {
            // Arrange
            var fileSystem = new MockFileSystem();

            // Act
            TestDelegate action = () => fileSystem.File.Create("C:\\Path\\NotFound.ext");

            // Assert
            Assert.IsFalse(fileSystem.Directory.Exists("C:\\path"));
            var exception = Assert.Throws<DirectoryNotFoundException>(action);
            Assert.That(exception.Message, Does.StartWith("Could not find a part of the path"));
        }

        [Test]
        public void Mockfile_CreateOverload_CanWriteToNewStream()
        {
            string fullPath = XFS.Path(@"c:\something\demo.txt");
            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(XFS.Path(@"c:\something"));
            var data = new UTF8Encoding(false).GetBytes("Test string");

            var sut = new MockFile(fileSystem);
            FileOptions fileOption = FileOptions.WriteThrough;

            using (var stream = sut.Create(fullPath, data.Length, fileOption))
            {
                stream.Write(data, 0, data.Length);
            }

            var mockFileData = fileSystem.GetFile(fullPath);
            var fileData = mockFileData.Contents;

            Assert.That(fileData, Is.EqualTo(data));
        }

        [Test]
        public void Mockfile_CreateOverload_OverwritesExistingFile()
        {           
            
        }

        [Test]
        public void Mockfile_CreateOverload_ShouldCreateNewStream()
        {
            string fullPath = XFS.Path(@"c:\something\demo.txt");
            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory(XFS.Path(@"c:\something"));            
            FileOptions fileOption = FileOptions.WriteThrough;
            var mockFile = new MockFile(fileSystem);
            Assert.That(fileSystem.FileExists(fullPath), Is.False);
            mockFile.Create(fullPath, 20, fileOption).Dispose();
            Assert.That(fileSystem.FileExists(fullPath), Is.True);
        }

        [Test]
        public void Mockfile_CreateOverload_ShouldThrowArgumentExceptionIfPathIsZeroLength()
        {



        }

        [Test]
        public void Mockfile_CreateOverload_ShouldThrowArgumentExceptionIfPathIsNull()
        {

        }

        [Test]        
        public void MockFile_CreateOverload_ShouldThrowDirectoryNotFoundExceptionIfCreatingAndParentPathDoesNotExist()
        {

        }

    }
}