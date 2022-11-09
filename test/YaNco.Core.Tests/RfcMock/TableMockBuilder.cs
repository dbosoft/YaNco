using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Dbosoft.YaNco;
using LanguageExt;
using Moq;

namespace YaNco.Core.Tests.RfcMock
{
    [ExcludeFromCodeCoverage]
    public class TableMockBuilder
    {
        public List<Mock<IStructure>> Structures { get; private set; } = new List<Mock<IStructure>>();
        public Mock<ITable> Table { get; }

        public TableMockBuilder(Mock<ITable> tableMock)
        {
            Table = tableMock;
        }

        public TableMockBuilder AddRow(Action<Mock<IStructure>> structureBuilder)
        {
            var structure = new Mock<IStructure>();
            structure.SetReturnsDefault(Prelude.Right<RfcErrorInfo, Unit>(Unit.Default));

            structureBuilder(structure);

            Structures.Add(structure);

            return this;
        }
    }
}