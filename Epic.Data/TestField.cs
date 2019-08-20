using System;
using System.Collections.Generic;
using System.Text;
using HexoGrid;

namespace Epic.Data
{
	public class FieldCell
	{
		
	}

    public class TestField
    {
	    private readonly HexGrid<FieldCell> _grid;

	    public TestField(int width, int height)
	    {
		    _grid = new HexGrid<FieldCell>(width, height, HexGridType.HorizontalEven);
	    }
    }
}
