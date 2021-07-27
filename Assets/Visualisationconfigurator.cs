using IATK;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
    public class Visualisationconfigurator : AbstractVisualisation
    {
        public List<string> selectedstates;
        public Visualisation visualisation;

        private View CreateSimpleVisualisation(Visualisation visualisationReference, CreationConfiguration configuration)
        {

            if (visualisationReference.dataSource != null)
            {
                if (!visualisationReference.dataSource.IsLoaded) visualisationReference.dataSource.load();

                ViewBuilder builder = new ViewBuilder(geometryToMeshTopology(configuration.Geometry), "Simple Visualisation");

                if ((visualisationReference.dataSource[getAxis(configuration.Axies, CreationConfiguration.Axis.X)] != null) ||
                    (visualisationReference.dataSource[getAxis(configuration.Axies, CreationConfiguration.Axis.Y)] != null) ||
                    (visualisationReference.dataSource[getAxis(configuration.Axies, CreationConfiguration.Axis.Z)] != null))
                {
                    builder.initialiseDataView(visualisationReference.dataSource.DataCount);

                    if (visualisationReference.dataSource[getAxis(configuration.Axies, CreationConfiguration.Axis.X)] != null)
                        builder.setDataDimension(visualisationReference.dataSource[getAxis(configuration.Axies, CreationConfiguration.Axis.X)].Data, ViewBuilder.VIEW_DIMENSION.X);
                    if (visualisationReference.dataSource[getAxis(configuration.Axies, CreationConfiguration.Axis.Y)] != null)
                        builder.setDataDimension(visualisationReference.dataSource[getAxis(configuration.Axies, CreationConfiguration.Axis.Y)].Data, ViewBuilder.VIEW_DIMENSION.Y);
                    if (visualisationReference.dataSource[getAxis(configuration.Axies, CreationConfiguration.Axis.Z)] != null)
                        builder.setDataDimension(visualisationReference.dataSource[getAxis(configuration.Axies, CreationConfiguration.Axis.Z)].Data, ViewBuilder.VIEW_DIMENSION.Z);

                    //destroy the view to clean the big mesh

                    //return the appropriate geometry view
                    return ApplyGeometryAndRendering(creationConfiguration, ref builder);
                }

            }

            return null;

        }

        protected string getAxis(Dictionary<CreationConfiguration.Axis, string> axies, CreationConfiguration.Axis axis)
        {

            string axes = null;
            string retVal = "";
            if (axies.TryGetValue(axis, out axes))
                retVal = axes;

            return retVal;
        }
        // Start is called before the first frame update
        void Start()
        {

            DataSource data = visualisation.dataSource;
            
            Dictionary<CreationConfiguration.Axis, string> axies = new Dictionary<CreationConfiguration.Axis, string>();
            if (visualisation.xDimension.Attribute != "" && visualisation.xDimension.Attribute != "Undefined")
            {
                axies.Add(CreationConfiguration.Axis.X, "year");
            }
            if (visualisation.yDimension.Attribute != "" && visualisation.yDimension.Attribute != "Undefined")
            {
            axies.Add(CreationConfiguration.Axis.Y, "insgesamt1");
            }
            if (visualisation.zDimension.Attribute != "" && visualisation.zDimension.Attribute != "Undefined")
            {
                axies.Add(CreationConfiguration.Axis.Z, "insgesamt2");
            }


            if (creationConfiguration == null)
                creationConfiguration = new CreationConfiguration(visualisation.geometry, axies);
            else
            {
 
                creationConfiguration.Axies = axies;
                creationConfiguration.Geometry = visualisationReference.geometry;
                creationConfiguration.LinkingDimension = visualisationReference.linkingDimension;
                creationConfiguration.Size = visualisationReference.size;
                creationConfiguration.MinSize = visualisationReference.minSize;
                creationConfiguration.MaxSize = visualisationReference.maxSize;
                creationConfiguration.colour = visualisationReference.colour;
            }

            _ = CreateSimpleVisualisation(visualisation, creationConfiguration);

        }

        // Update is called once per frame
        void Update()
        {

        }

    public override void CreateVisualisation()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateVisualisation(PropertyType propertyType)
    {
        throw new System.NotImplementedException();
    }

    public override void SaveConfiguration()
    {
        throw new System.NotImplementedException();
    }

    public override void LoadConfiguration()
    {
        throw new System.NotImplementedException();
    }

    public override Color[] mapColoursContinuous(float[] data)
    {
        throw new System.NotImplementedException();
    }
}