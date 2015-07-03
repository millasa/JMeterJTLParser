# JMeterJTLParser
Parser for JMeter JTL (XML) file. Creates TXT file with aggregated values from JTL.
Results are present as table with a header ("URL", "Avg", "Max" and "Min"), tabs are used as delimiters (easy to convert to CSV).

##Running the application

To launch the application use:

*report_parser.exe jtl_file_name_without_extension unique_identifier* 

**NOTE:** use date, revision number or ID as unique identifier
