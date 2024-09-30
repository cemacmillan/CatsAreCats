<?xml version="1.0" encoding="utf-8"?>
<Patch>

  <!-- Erin's Cat Overhaul Loaded -->
  <Operation Class="PatchOperationFindMod">
    <mods>
      <li>Erin's Cat Overhaul</li>
    </mods>

    <match Class="PatchOperationConditional">

      <!-- Only add our comp if Erin's Cat Overhaul is loaded -->
      <xpath>Defs/ThingDef[defName="Cat"]/comps/li[@Class='DIL_CatsAreCats.CompProperties_VerbHandler']</xpath>
      <nomatch Class="PatchOperationAdd">
        <xpath>Defs/ThingDef[defName="Cat"]/comps</xpath>
        <value>
          <li Class="DIL_CatsAreCats.CompProperties_VerbHandler">
            <rangedProbability>0.7</rangedProbability>
          </li>
        </value>
      </nomatch>

    </match>
  </Operation>

  <!-- No preceding Cat mod has given us comps. I guess we have to do it ourselves. -->
  <Operation Class="PatchOperationConditional">
    
   
    <xpath>Defs/ThingDef[defName="Cat"]/comps</xpath>
    <nomatch Class="PatchOperationAdd">
      <!-- If no <comps>, add the entire <comps> section with our comp -->
      <xpath>Defs/ThingDef[defName="Cat"]</xpath>
      <value>
        <comps>
          <li Class="DIL_CatsAreCats.CompProperties_VerbHandler">
            <rangedProbability>0.7</rangedProbability>
          </li>
        </comps>
      </value>
    </nomatch>

    <!-- If <comps> exists but our comp is missing, just add the comp
    <match Class="PatchOperationConditional">
      <xpath>Defs/ThingDef[defName="Cat"]/comps/li[@Class='DIL_CatsAreCats.CompProperties_VerbHandler']</xpath>
      <nomatch Class="PatchOperationAdd">
        <xpath>Defs/ThingDef[defName="Cat"]/comps</xpath>
        <value>
          <li Class="DIL_CatsAreCats.CompProperties_VerbHandler">
            <rangedProbability>0.7</rangedProbability>
          </li>
        </value>
      </nomatch>
    </match>
     -->

  </Operation>

</Patch>