/* Generate measures from columns */

var MeasuresTable = Model.Tables["_Measures"];
var kpiSourceTable = Model.Tables["Sales"];

foreach(var column in kpiSourceTable.Columns)
{
    if (column.Name.Contains("KPI ")) 
    {
        var new_measure = MeasuresTable.AddMeasure(
        column.Name + " BASE",                                    // Name
        "SUM(" + column.DaxObjectFullName + ")",        // DAX expression
        "Generate from Columns\\BASE"                         // Display Folder
        );
    }
}

/* Delete measures */

var MeasuresTable = Model.Tables["_Measures"];
foreach(var m in MeasuresTable.Measures.Where(m => (m.DisplayFolder.Contains("TO DELETE"))).ToList())
{
    m.Delete();
}

/* Description */

var MeasuresTable = Model.Tables["_Measures"];
foreach(var m in MeasuresTable.Measures.ToList())
{
    m.Description = m.Expression;
}

/* Generate measures from another measures */

var MeasuresTable = Model.Tables["_Measures"];
var baseMeasures = MeasuresTable.Measures.Where(m => (m.DisplayFolder=="Generate from Columns\\BASE")).ToList();

foreach(var measure in baseMeasures)
{
    // get correct measure name
    var indexOfBase = measure.Name.IndexOf("BASE");
    var correctMeasureName = measure.Name.Substring(0, indexOfBase);
    
    // add AC measure
    var new_AC_measure = MeasuresTable.AddMeasure(
    correctMeasureName + "AC",                      // Name
    measure.DaxObjectFullName,                       // DAX expression
    "Generate from BASE\\AC"                         // Display Folder
    );

    // add PY measure
    var new_PY_measure = MeasuresTable.AddMeasure(
    correctMeasureName + "PY",                                                       // Name
    "CALCULATE(" + measure.DaxObjectFullName + ", SAMEPERIODLASTYEAR('Date'[Date]))", // DAX expression
    "Generate from BASE\\PY"                                                          // Display Folder
    );
}

/* Create measures from calculation items */

var MeasuresTable = Model.Tables["_Measures"];
var baseMeasures = MeasuresTable.Measures.Where(m => (m.DisplayFolder=="Generate from Columns\\BASE")).ToList();

foreach(var measure in baseMeasures)
{
    // get correct measure name
    var indexOfBase = measure.Name.IndexOf("BASE");
    var measureNameWithoutBase = measure.Name.Substring(0, indexOfBase);
    var correctMeasureName = measureNameWithoutBase.Remove(0,3);
    
    foreach(var calcItem in (Model.Tables["Time Intelligence"] as CalculationGroupTable).CalculationItems)
    {
        // add measure
        var new_measure = MeasuresTable.AddMeasure(
        correctMeasureName + " " + calcItem.Name,                              // Name
        "CALCULATE(" + measure.DaxObjectFullName + ", 'Time Intelligence'[TimeIntelligence]=\"" + calcItem.Name + "\")",   // DAX expression
        "Generate from calculation items\\" + calcItem.Name                         // Display Folder
    );
    }
}

/* Create measures from calculation items + Description*/

var MeasuresTable = Model.Tables["_Measures"];
var baseMeasures = MeasuresTable.Measures.Where(m => (m.DisplayFolder=="Generate from Columns\\BASE")).ToList();

foreach(var measure in baseMeasures)
{
    // get correct measure name
    var correctMeasureName = measure.Name.Remove(0,3);
    
    foreach(var calcItem in (Model.Tables["Time Intelligence"] as CalculationGroupTable).CalculationItems)
    {
        // add measure
        var new_measure = MeasuresTable.AddMeasure(
        correctMeasureName + " " + calcItem.Name,                              // Name
        "CALCULATE(" + measure.DaxObjectFullName + ", 'Time Intelligence'[TimeIntelligence]=\"" + calcItem.Name + "\")",   // DAX expression
        "Generate from calculation items\\" + calcItem.Name                         // Display Folder
        );
        new_measure.Description = "BASE: " + measure.Expression + @"
        
CALC: " + calcItem.Expression;
    }
}



/* Hide Measures */

var MeasuresTable = Model.Tables["_Measures"];
foreach(var m in MeasuresTable.Measures.Where(m => (m.DisplayFolder.Contains("BASE"))).ToList())
{
    m.IsHidden = true;
}

/* Format Measures */

var MeasuresTable = Model.Tables["_Measures"];
foreach(var m in MeasuresTable.Measures.Where(m => (m.DisplayFolder.Contains("%"))).ToList())
{
    m.FormatString = "#.##%";
}




/* Create measures from calculation items */

var MeasuresTable = Model.Tables["_Measures"];
var baseMeasures = MeasuresTable.Measures.Where(m => (m.DisplayFolder=="Generate from BASE\\BASE")).ToList();

foreach(var measure in baseMeasures)
{
    // get correct measure name
    var indexOfBase = measure.Name.IndexOf("BASE");
    var correctMeasureName = measure.Name.Substring(0, indexOfBase);
    
    foreach(var calcItem in (Model.Tables["Time Intelligence"] as CalculationGroupTable).CalculationItems)
    {
        // add measure
        var new_measure = MeasuresTable.AddMeasure(
        correctMeasureName + calcItem.Name,                              // Name
        "CALCULATE(" + measure.DaxObjectFullName + ", 'Time Intelligence'[TimeIntelligence]=\"" + calcItem.Name + "\")",   // DAX expression
        "Generate from calculation items\\" + calcItem.Name                         // Display Folder
    );
    }
}

/* Calculation Group

AC
SELECTEDMEASURE()

PY
CALCULATE(SELECTEDMEASURE(), SAMEPERIODLASTYEAR('Date'[Date]))


AC-PY %
VAR ac_ = CALCULATE(SELECTEDMEASURE(), 'Time Intelligence'[TimeIntelligence]= "AC")
VAR py_ = CALCULATE(SELECTEDMEASURE(), 'Time Intelligence'[TimeIntelligence]= "PY")

RETURN
DIVIDE(ac_ - py_, py_)


PM
CALCULATE(SELECTEDMEASURE(), PREVIOUSMONTH('Date'[Date]))

/*